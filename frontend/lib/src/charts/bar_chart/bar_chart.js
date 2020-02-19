class BarChart {

    _BAR_COLORS = [
        "#396AB1",
        "#DA7C30",
        "#3E9651",
        "#CC2529",
        "#535154",
        "#6B4C9A",
        "#922428",
        "#948B3D"
    ];

    _ELEMENT_HEIGHT = 400;
    _CHART_PADDING_VERTICAL = 30;
    _CHART_PADDING_HORIZONTAL = 30;

    create(selector, dataSet) {
        this._removeOld(selector);

        let element = document.getElementsByClassName("tab-content")[0];
        if (element == null)
            return;

        let elementWidth = element.clientWidth;
        let chartWidth = elementWidth - this._CHART_PADDING_HORIZONTAL * 2;
        let chartHeight = this._ELEMENT_HEIGHT - this._CHART_PADDING_VERTICAL * 2;

        let barData = this._createBarData(dataSet);

        let xScale = this._createXScale(chartWidth, barData);
        let yScale = this._createYScale(chartHeight, barData);
        let colorScale = this._createColorScale(dataSet, this._BAR_COLORS);

        let svg = this._createSvg(selector, chartWidth, this._ELEMENT_HEIGHT, this._CHART_PADDING_HORIZONTAL, this._CHART_PADDING_VERTICAL);

        this._createGridYLines(svg, yScale, chartWidth);
        this._createXAxis(svg, xScale, chartHeight);
        this._createYAxis(svg, yScale);
        this._createBars(svg, barData, xScale, yScale, colorScale, chartHeight);

        this._createMouseHoverDivs(svg, selector, yScale);
    }

    _removeOld(selector) {
        d3.select(selector).selectAll("*").remove();
        d3.select(selector).html("");
    }

    _createBarData(dataSet) {
        let data = [];
        Object.keys(dataSet).forEach((key, index, map) => {
            data.push({'key': key, 'value': dataSet[key]});
        });

        return data;
    }

    _createXScale(width, data) {
        return d3.scaleBand()
            .range([0, width])
            .domain(data.map(d => d.key))
            .padding(0.1);
    }

    _createYScale(height, data) {
        return d3.scaleLinear()
            .range([height, 0])
            .domain([0, d3.max(data, datum => datum.value)]);
    }

    _createSvg(selector, width, height, paddingLeft, paddingTop) {
        return d3.select(selector)
            .append("svg")
            .attr("width", "100%")
            .attr("height", height)
            .attr("class", "bar-chart")
            .append("g")
            .attr("transform", "translate(" + paddingLeft + "," + paddingTop + ")");
    }

    _createBackground(svg, width) {
        svg.append("rect")
            .attr("class", "overlay")
            .attr("width", width)
            .attr("height", this._ELEMENT_HEIGHT);
    }

    _createColorScale(dataSet, colors) {
        return d3.scaleOrdinal()
            .domain(dataSet)
            .range(colors);
    }

    _createGridYLines(svg, yScale, chartWidth) {
        svg.append("g")
            .attr("class", "y grid")
            .call(d3.axisLeft(yScale).tickSize(-chartWidth).tickFormat(""));
    }

    _createXAxis(svg, xScale, height) {
        svg.append("g")
            .attr("transform", "translate(0," + height + ")")
            .call(d3.axisBottom(xScale));
    }

    _createYAxis(svg, yScale) {
        svg.append("g")
            .call(d3.axisLeft(yScale));
    }

    _createBars(svg, data, xScale, yScale, colorScale, height) {
        console.log(yScale);
        svg.selectAll(".bar")
            .data(data)
            .enter()
            .append("rect")
            .attr("class", "bar")
            .style("fill", d => colorScale(d.key))
            .attr("x", (d) => { console.log(d); return xScale(d.key); })
            .attr("width", xScale.bandwidth())
            .attr("y", function(d) { console.log("AAA"); console.log(d.value); console.log(yScale(d.value)); return yScale(d.value); })
            .attr("height", (d) => { console.log("----"); console.log(d.value); console.log(yScale(d.value)); return height - yScale(d.value)});

        let bars = svg.selectAll(".bar");
        bars
            .on('mouseover', (_, hoveredElementIndex) => {
                bars.transition().style("opacity", 0.5);
                d3.select(bars.nodes()[hoveredElementIndex]).transition().style("opacity", 1);
            })
            .on('mouseout', (d, i) => {
                bars.transition().style("opacity", 1.0);
            });
    }

    _createMouseHoverDivs(svg, containerSelector, yScale) {
        let mouseHoverDiv = d3.select(containerSelector).append("div")
            .attr("class", "tooltip")
            .style("opacity", 0);

        let horizontalMarker = d3.select(containerSelector).append("div")
            .attr("class", "position-marker horizontal")
            .style("opacity", 0);

        let self = this;
        d3.select(containerSelector)
            .on('mouseover', () => {
                mouseHoverDiv.style("opacity", 1);
                horizontalMarker.style("opacity", 1);
            })
            .on('mouseout', () => {
                mouseHoverDiv.style("opacity", 0);
                horizontalMarker.style("opacity", 0);
            })
            .on('mousemove', function (d, _) {
                mouseHoverDiv.style("left", (d3.event.layerX - mouseHoverDiv._groups[0][0].clientWidth) + "px").style("top", (d3.event.layerY) + "px");
                horizontalMarker.style("top", (d3.event.layerY) + "px");

                let mouseChartPosition = d3.mouse(this)[1];
                let tooltipHtml = self._getValueAtPosition(yScale, mouseChartPosition);
                mouseHoverDiv.style("opacity", tooltipHtml === ""? 0 : 1);
                if (tooltipHtml !== "") {
                    mouseHoverDiv.html(tooltipHtml);
                }
            });
    }

    _getValueAtPosition(yScale, yPosition) {
        return yScale.invert(yPosition - this._CHART_PADDING_VERTICAL).toFixed(2);
    }

}