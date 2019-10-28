let Socneto = {

    Charts: {
        createLineChart: function (selector, dataSets, dataLabels) {
            let lineChart = new LineChart();
            lineChart.create(selector, dataSets, dataLabels);
        },

        createPieChart: function (selector, dataSets, dataLabels) {
            let pieChart = new PieChart();
            pieChart.create(selector, dataSets, dataLabels);
        }
    }

};

window.Socneto = Socneto;

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
            this._createChartLegend(svg, dataSets[index], dataLabels[index], color, curve, index);
        }

        this._createMouseHoverDivs(background, selector, xScale, dataSets, dataLabels);
    }

    _removeOld(selector) {
        d3.select(selector).select("svg").remove();
    }

    _createXScale(dataSets, width) {
        return d3.scaleTime()
            .domain(d3.extent(dataSets.flat(), function(datum) { return new Date(datum.x); }))
            .range([0, width]);
    }

    _createYScale(dataSets, height) {
        return d3.scaleLinear()
            .domain(d3.extent(dataSets.flat(), function(datum) { return datum.y; }))
            .range([height, 0]);
    }

    _createChartCurve(xScale, yScale) {
        return d3.line()
            .x(function (datum, _) {
                return xScale(new Date(datum.x));
            })
            .y(function (datum) {
                return yScale(datum.y);
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
    }

    _createChartLegend(svg, dataSet, dataLabel, color, curve, index) {
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
            .attr("class", "position-marker vertical")
            .style("opacity", 0);

        let horizontalMarker = d3.select(containerSelector).append("div")
            .attr("class", "position-marker horizontal")
            .style("opacity", 0);

        let self = this;
        d3.select(containerSelector)
            .on('mouseover', function (d, i) {
                mouseHoverDiv.transition().duration(100).style("opacity", 1);
                verticalMarker.transition().duration(100).style("opacity", 1);
                horizontalMarker.transition().duration(100).style("opacity", 1);
            })
            .on('mouseout', function (d, i) {
                mouseHoverDiv.transition().duration(100).style("opacity", 0);
                verticalMarker.transition().duration(100).style("opacity", 0);
                horizontalMarker.transition().duration(100).style("opacity", 0);
            })
            .on('mousemove', function (d, _) {
                mouseHoverDiv.style("left", (d3.event.layerX) + "px").style("top", (d3.event.layerY) + "px");
                verticalMarker.style("left", (d3.event.layerX) + "px");

                horizontalMarker.style("top", (d3.event.layerY) + "px");

                let mouseChartPosition = d3.mouse(this)[0] - self._LEGEND_WIDTH;
                let tooltipHtml = self._createMouseHoverTooltipHtml(mouseChartPosition, xScale, dataSets, dataLabels);
                mouseHoverDiv.html(tooltipHtml);
            });
    }

    _createMouseHoverTooltipHtml(position, xScale, dataSets, dataLabels) {
        let currentValues = this._getValuesAtPosition(position, xScale, dataSets, dataLabels);
        currentValues.sort(function (a, b) { return b["value"] - a["value"] });
        let tooltipHtml = "";
        for (let index = 0; index < currentValues.length; index++) {
            let label = currentValues[index]["label"];
            let currentValue = currentValues[index]["value"];
            tooltipHtml = tooltipHtml.concat(`<span style="color: ${currentValues[index]["color"]}">${label}:</span> ${currentValue.toFixed(2)}<br>`);
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
                if (new Date(datum.x) > x) {
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
        let xTimeMillis = currentX.getTime();
        let previousTimeMillis = new Date(previousDatum.x).getTime();
        let nextTimeMillis = new Date(nextDatum.x).getTime();
        let positionBetween = (xTimeMillis - previousTimeMillis) / (nextTimeMillis - previousTimeMillis);

        let valuesDiff = nextDatum.y - previousDatum.y;
        let valueInterpolation = positionBetween * valuesDiff;

        return previousDatum.y + valueInterpolation;
    }

}

class PieChart {

    _PIE_COLORS = [
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
    _CHART_PADDING_VERTICAL = 20;
    _CHART_PADDING_HORIZONTAL = 10;

    create(selector, dataSet) {
        this._removeOld(selector);

        let elementWidth = document.getElementsByClassName("tab-content")[0].clientWidth;
        let chartWidth = elementWidth - this._LEGEND_WIDTH - this._CHART_PADDING_HORIZONTAL;
        let chartHeight = this._ELEMENT_HEIGHT - this._CHART_PADDING_VERTICAL;

        let radius = Math.min(chartWidth, chartHeight) / 2;

        let svg = d3.select(selector)
            .append("svg")
            .attr("width", chartWidth)
            .attr("height", this._ELEMENT_HEIGHT)
            .attr("class", "pie-chart")
            .append("g")
            .attr("transform", "translate(" + chartWidth / 2 + "," + this._ELEMENT_HEIGHT / 2 + ")");

        let totalSum = Object.values(dataSet).reduce((t, n) => t + n);

        let color = d3.scaleOrdinal()
            .domain(dataSet)
            .range(this._PIE_COLORS);

        let pie = d3.pie()
            .value(d => d.value);

        let data_ready = pie(d3.entries(dataSet));

        let arc = d3.arc().innerRadius(radius).outerRadius(radius / 5);

        svg.append('g').attr('class', 'slices');
        svg.append("g").attr("class", 'labels');

        let slice = svg.select(".slices").selectAll("path.slice").data(data_ready);
        slice.enter()
                .insert("path")
                .style("fill", function(d) { return color(d.data.key); })
                .style("stroke", "white")
                .attr("class", "slice")
                .attr('d', arc)
            .exit()
            .remove();


        svg.select(".labels").attr("text-anchor", "middle");
        let label = svg.select(".labels").selectAll("text").data(data_ready);
        label.enter()
                .append("text")
                .attr("dy", ".35em")
                .attr("transform", d => {
                    let pos = arc.centroid(d).map(d => d * 1.2);
                    return "translate("+ pos +")";
                });


        svg.select(".labels").selectAll("text").insert("tspan")
            .text(d => d.data.key)
            .attr("class", "label")
            .attr("y", "-0.7em");

        svg.select(".labels").selectAll("text").insert("tspan")
            .text(d => ((d.data.value / totalSum) * 100).toFixed(1) + "% (" + d.data.value + ")")
            .attr("class", "value")
            .attr("x", 0)
            .attr("y", "0.7em")
    }

    _removeOld(selector) {
        d3.select(selector).select("svg").remove();
    }

}