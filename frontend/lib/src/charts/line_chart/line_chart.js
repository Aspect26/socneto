class LineChart {

    _LINE_COLORS = [
        "#396AB1",
        "#DA7C30",
        "#3E9651",
        "#CC2529",
        "#535154",
        "#6B4C9A",
        "#922428",
        "#948B3D"
    ];

    _LEGEND_WIDTH = 150;
    _ELEMENT_HEIGHT = 450;
    _CHART_PADDING_BOTTOM = 20;
    _CHART_PADDING_RIGHT = 10;

    _isXDate = false;

    create(selector, dataSets, dataLabels, isXDate) {
        this._removeOld(selector);
        this._isXDate = isXDate;

        let element = document.getElementsByClassName("tab-content")[0];
        if (element == null)
            return;

        let elementWidth = element.clientWidth;
        let chartWidth = elementWidth - this._LEGEND_WIDTH - this._CHART_PADDING_RIGHT;
        let chartHeight = this._ELEMENT_HEIGHT - this._CHART_PADDING_BOTTOM;

        let xScale = this._createXScale(dataSets, chartWidth);
        let yScale = this._createYScale(dataSets, chartHeight);
        let curve = this._createChartCurve(xScale, yScale);

        let svg = this._createSvg(selector, elementWidth);
        let background = this._createBackground(svg, elementWidth);
        this._createXAxis(svg, xScale, chartHeight);
        this._createYAxis(svg, yScale);

        for (let index = 0; index < dataSets.length; ++index) {
            let color = this._LINE_COLORS[index % this._LINE_COLORS.length];
            this._createChartLine(svg, dataSets[index], dataLabels[index], color, curve);
            this._createChartLegend(svg, dataSets[index], dataLabels[index], color, curve, index);
        }

        this._createMouseHoverDivs(background, selector, xScale, dataSets, dataLabels);
    }

    _removeOld(selector) {
        d3.select(selector).selectAll("*").remove();
        d3.select(selector).html("");
    }

    _createXScale(dataSets, width) {
        if (this._isXDate) {
            return d3.scaleTime()
                .domain(d3.extent(dataSets.flat(), datum => new Date(datum.x)))
                .range([0, width]);
        } else {
            return d3.scaleLinear()
                .domain(d3.extent(dataSets.flat(), datum => datum.x))
                .range([0, width])
        }
    }

    _createYScale(dataSets, height) {
        return d3.scaleLinear()
            .domain(d3.extent(dataSets.flat(), datum => datum.y))
            .range([height, 0]);
    }

    _createChartCurve(xScale, yScale) {
        return d3.line()
            .x((datum, _) => xScale(this._isXDate? new Date(datum.x) : datum.x))
            .y(datum => yScale(datum.y));
    }

    _createSvg(selector, width) {
        return d3.select(selector)
            .append("svg")
            .attr("width", "100%")
            .attr("height", this._ELEMENT_HEIGHT)
            .attr("class", "line-chart")
            .append("g")
    }

    _createBackground(svg, width) {
        svg.append("rect")
            .attr("class", "overlay")
            .attr("width", width)
            .attr("height", this._ELEMENT_HEIGHT);
    }

    _createXAxis(svg, xScale, chartHeight) {
        svg.append("g")
            .attr("class", "x axis")
            .attr("transform", "translate(" + this._LEGEND_WIDTH + "," + chartHeight + ")")
            .call(d3.axisBottom(xScale));
    }

    _createYAxis(svg, yScale) {
        svg.append("g")
            .attr("class", "y axis")
            .attr("transform", "translate(" + this._LEGEND_WIDTH + ", 0)")
            .call(d3.axisLeft(yScale));
    }

    _createChartLine(svg, dataSet, dataLabel, color, curve) {
        svg.append("path")
            .datum(dataSet)
            .attr("class", "line")
            .style('stroke', color)
            .attr("transform", "translate(" + this._LEGEND_WIDTH + ", 0)")
            .attr("d", curve);
    }

    _createChartLegend(svg, dataSet, dataLabel, color, curve, index) {
        let legend = svg.append("g").attr("transform", "translate(0, " + 30 + ")").attr("class", "legend-entry");
        legend.append("circle").attr("cx", 10).attr("cy", index * 20).attr("r", 6).style("fill", color);
        legend.append("text").attr("x", 20).attr("y", index * 20).text(dataLabel).style("font-size", "15px").attr("alignment-baseline","middle");

        legend
            .on("mouseover", function(d, i) {
                // path.node().classList.add("selected");
                legend.node().classList.add("selected");
            })
            .on("mouseout", function(d, i) {
                // path.node().classList.remove("selected");
                legend.node().classList.remove("selected");
            });
    }

    _createMouseHoverDivs(svg, containerSelector, xScale, dataSets, dataLabels) {
        let mouseHoverDiv = d3.select(containerSelector).append("div")
            .attr("class", "tooltip")
            .style("opacity", 0);

        let verticalMarker = d3.select(containerSelector).append("div")
            .attr("class", "position-marker vertical")
            .style("opacity", 0);

        let horizontalMarker = d3.select(containerSelector).append("div")
            .attr("class", "position-marker horizontal")
            .style("opacity", 0);

        let self = this;
        d3.select(containerSelector)
            .on('mouseover', () => {
                mouseHoverDiv.style("opacity", 1);
                verticalMarker.style("opacity", 1);
                horizontalMarker.style("opacity", 1);
            })
            .on('mouseout', () => {
                mouseHoverDiv.style("opacity", 0);
                verticalMarker.style("opacity", 0);
                horizontalMarker.style("opacity", 0);
            })
            .on('mousemove', function (d, _) {
                mouseHoverDiv.style("left", (d3.event.layerX - mouseHoverDiv._groups[0][0].clientWidth) + "px").style("top", (d3.event.layerY) + "px");
                verticalMarker.style("left", (d3.event.layerX) + "px");
                horizontalMarker.style("top", (d3.event.layerY) + "px");

                let mouseChartPosition = d3.mouse(this)[0] - self._LEGEND_WIDTH;
                let tooltipHtml = self._createMouseHoverTooltipHtml(mouseChartPosition, xScale, dataSets, dataLabels);
                mouseHoverDiv.style("opacity", tooltipHtml === ""? 0 : 1);
                if (tooltipHtml !== "") {
                    mouseHoverDiv.html(tooltipHtml);
                }
            });
    }

    _createMouseHoverTooltipHtml(position, xScale, dataSets, dataLabels) {
        let currentValues = this._getValuesAtPosition(position, xScale, dataSets, dataLabels);
        currentValues.sort(function (a, b) { return b["value"] - a["value"] });
        let tooltipHtml = "";
        for (let index = 0; index < currentValues.length; index++) {
            let label = currentValues[index]["label"];
            let currentValue = currentValues[index]["value"];
            let currentValueHTML = currentValue != null? `<span style="color: ${currentValues[index]["color"]}">${label}:</span> ${currentValue.toFixed(2)}<br>` : "";
            tooltipHtml = tooltipHtml.concat(currentValueHTML);
        }

        return tooltipHtml;
    }

    _getValuesAtPosition(position, xScale, dataSets, dataLabels) {
        let x = xScale.invert(position);
        let currentValues = [];

        for (let i = 0; i < dataSets.length; i++) {
            let dataSet = dataSets[i];
            let previousDatum = null;
            for (let j = 0; j < dataSet.length; j++) {
                let datum = dataSet[j];
                let currentX = this._isXDate? new Date(datum.x) : datum.x;
                if (currentX > x) {
                    let currentValue = datum.value;
                    if (previousDatum != null) {
                        currentValue = this._interpolateValue(x, previousDatum, datum);
                    }
                    currentValues.push({
                        "label": dataLabels[i],
                        "value": currentValue,
                        "color": this._LINE_COLORS[i]
                    });
                    break;
                }
                previousDatum = datum;
            }
        }

        return currentValues;
    }

    _interpolateValue(currentX, previousDatum, nextDatum) {
        let xValue = this._isXDate? currentX.getTime() : currentX;
        let previousXValue = this._isXDate? new Date(previousDatum.x).getTime() : previousDatum.x;
        let nextXValue = this._isXDate? new Date(nextDatum.x).getTime() : nextDatum.x;
        let positionBetween = (xValue - previousXValue) / (nextXValue - previousXValue);

        let valuesDiff = nextDatum.y - previousDatum.y;
        let valueInterpolation = positionBetween * valuesDiff;

        return previousDatum.y + valueInterpolation;
    }

}
