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

    _CHART_MARGIN = {
        top: 50,
        right: 50,
        bottom: 50,
        left: 50
    };

    _LEGEND_WIDTH = 150;
    _ELEMENT_HEIGHT = 450;

    create(selector, dataSets, dataLabels) {
        this._removeOld(selector);

        let elementWidth = document.getElementsByClassName("tab-content")[0].clientWidth;
        let chartWidth = elementWidth - this._CHART_MARGIN.left - this._CHART_MARGIN.right - this._LEGEND_WIDTH;
        let chartHeight = this._ELEMENT_HEIGHT - this._CHART_MARGIN.top - this._CHART_MARGIN.bottom;

        let xScale = this._createXScale(dataSets, chartWidth);
        let yScale = this._createYScale(dataSets, chartHeight);
        let curve = this._createChartCurve(xScale, yScale);

        let svg = this._createSvg(selector, elementWidth);
        this._createXAxis(svg, xScale, chartHeight);
        this._createYAxis(svg, yScale);

        for (let index = 0; index < dataSets.length; ++index) {
            let color = this._LINE_COLORS[index % this._LINE_COLORS.length];
            this._createChartLine(svg, dataSets[index], dataLabels[index], color, curve, index);
        }
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
            })
            .curve(d3.curveMonotoneX);
    }

    _createSvg(selector, width) {
        return d3.select(selector)
            .append("svg")
            .attr("width", width)
            .attr("height", this._ELEMENT_HEIGHT)
            .append("g")
            .attr("transform", "translate(" + this._CHART_MARGIN.left  + "," + this._CHART_MARGIN.top + ")");
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
        svg.append("path")
            .datum(dataSet)
            .attr("class", "line")
            .style('stroke', color)
            .attr("transform", "translate(" + this._LEGEND_WIDTH + ", 0)")
            .attr("d", curve);

        svg.append("circle").attr("cx", 0).attr("cy", index * 20).attr("r", 6).style("fill", color);
        svg.append("text").attr("x", 20).attr("y", index * 20).text(dataLabel).style("font-size", "15px").attr("alignment-baseline","middle")
    }
}

function createLineChart(selector, datasets, datalabels) {
    let lineChart = new LineChart();
    lineChart.create(selector, datasets, datalabels);
}