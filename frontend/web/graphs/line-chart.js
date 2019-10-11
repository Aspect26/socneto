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

    create(selector, dataSets, dataLabels) {
        this._removeOld(selector);

        let elementWidth = document.getElementsByClassName("tab-content")[0].clientWidth;
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
            this._createChartLine(svg, dataSets[index], dataLabels[index], color, curve, index);
        }

        this._createMouseHoverDivs(background, selector, xScale, dataSets, dataLabels);
    }

    _removeOld(selector) {
        d3.select(selector).select("svg").remove();
    }

    _createXScale(dataSets, width) {
        return d3.scaleTime()
            .domain(d3.extent(dataSets.flat(), function(datum) { return new Date(datum.date); }))
            .range([0, width]);
    }

    _createYScale(dataSets, height) {
        return d3.scaleLinear()
            .domain(d3.extent(dataSets.flat(), function(datum) { return datum.value; }))
            .range([height, 0]);
    }

    _createChartCurve(xScale, yScale) {
        return d3.line()
            .x(function (datum, _) {
                return xScale(new Date(datum.date));
            })
            .y(function (datum) {
                return yScale(datum.value);
            });
    }

    _createSvg(selector, width) {
        return d3.select(selector)
            .append("svg")
            .attr("width", width)
            .attr("height", this._ELEMENT_HEIGHT)
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

    _createChartLine(svg, dataSet, dataLabel, color, curve, index) {
        let path = svg.append("path")
            .datum(dataSet)
            .attr("class", "line")
            .style('stroke', color)
            .attr("transform", "translate(" + this._LEGEND_WIDTH + ", 0)")
            .attr("d", curve);

        let legend = svg.append("g").attr("transform", "translate(0, " + 30 + ")").attr("class", "legend-entry");
        legend.append("circle").attr("cx", 10).attr("cy", index * 20).attr("r", 6).style("fill", color);
        legend.append("text").attr("x", 20).attr("y", index * 20).text(dataLabel).style("font-size", "15px").attr("alignment-baseline","middle");

        legend
            .on("mouseover", function(d, i) {
                path.node().classList.add("selected");
                legend.node().classList.add("selected");
            })
            .on("mouseout", function(d, i) {
                path.node().classList.remove("selected");
                legend.node().classList.remove("selected");
            });
    }

    _createMouseHoverDivs(svg, containerSelector, xScale, dataSets, dataLabels) {
        let mouseHoverDiv = d3.select(containerSelector).append("div")
            .attr("class", "tooltip")
            .style("opacity", 0);


        let verticalMarker = d3.select(containerSelector).append("div")
            .attr("class", "vertical-marker")
            .style("opacity", 0);

        // let verticalMarkerSvg = svg.append("g")

        let self = this;
        d3.select(containerSelector)
            .on('mouseover', function (d, i) {
                mouseHoverDiv
                    .transition()
                    .duration(100)
                    .style("opacity", 1);

                verticalMarker
                    .transition()
                    .duration(100)
                    .style("opacity", 0)
            })
            .on('mouseout', function (d, i) {
                mouseHoverDiv.transition()
                    .duration(100)
                    .style("opacity", 0);

                verticalMarker.transition()
                    .duration(100)
                    .style("opacity", 0);
            })
            .on('mousemove', function (d, _) {
                mouseHoverDiv
                    .style("left", (d3.event.pageX) + "px")
                    .style("top", (d3.event.pageY) + "px");

                verticalMarker
                    .style("left", (d3.event.pageX) + "px");

                let mouseChartPosition = d3.mouse(this)[0] - self._LEGEND_WIDTH;
                let tooltipHtml = self._createMouseHoverTooltipHtml(mouseChartPosition, xScale, dataSets, dataLabels);
                mouseHoverDiv.html(tooltipHtml);
            });
    }

    _createMouseHoverTooltipHtml(position, xScale, dataSets, dataLabels) {
        let currentValues = this._getValuesAtPosition(position, xScale, dataSets, dataLabels);
        let tooltipHtml = "";
        for (let index = 0; index < currentValues.length; index++) {
            console.log(currentValues[index]);
            let label = currentValues[index]["label"];
            let currentValue = currentValues[index]["value"];
            tooltipHtml = tooltipHtml.concat(`<span style="color: ${this._LINE_COLORS[index]}">${label}:</span> ${currentValue.toFixed(2)}<br>`);
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
                if (new Date(datum.date) > x) {
                    let currentValue = datum.value;
                    if (previousDatum != null) {
                        currentValue = this._interpolateValue(x, previousDatum, datum);
                    }
                    currentValues.push({
                        "label": dataLabels[i],
                        "value": currentValue
                    });
                    break;
                }
                previousDatum = datum;
            }
        }

        return currentValues;
    }

    _interpolateValue(currentX, previousDatum, nextDatum) {
        let xTimeMillis = currentX.getTime();
        let previousTimeMillis = new Date(previousDatum.date).getTime();
        let nextTimeMillis = new Date(nextDatum.date).getTime();
        let positionBetween = (xTimeMillis - previousTimeMillis) / (nextTimeMillis - previousTimeMillis);

        let valuesDiff = nextDatum.value - previousDatum.value;
        let valueInterpolation = positionBetween * valuesDiff;

        return previousDatum.value + valueInterpolation;
    }

}

function createLineChart(selector, datasets, datalabels) {
    let lineChart = new LineChart();
    lineChart.create(selector, datasets, datalabels);
}