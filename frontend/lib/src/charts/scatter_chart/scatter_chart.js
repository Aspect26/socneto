class ScatterChart {

    _SCATTER_COLOR = "#396AB1";

    _ELEMENT_HEIGHT = 450;
    _CHART_PADDING_VERTICAL = 20;
    _CHART_PADDING_HORIZONTAL = 50;

    create(selector, dataSet) {
        this._removeOld(selector);

        let element = document.getElementsByClassName("tab-content")[0];
        if (element == null)
            return;

        let elementWidth = element.clientWidth;
        let chartWidth = elementWidth - this._CHART_PADDING_HORIZONTAL;
        let chartHeight = this._ELEMENT_HEIGHT - this._CHART_PADDING_VERTICAL;

        let xScale = this._createXScale(chartWidth, dataSet);
        let yScale = this._createYScale(chartHeight, dataSet);

        let svg = this._createSvg(selector, chartWidth, this._ELEMENT_HEIGHT, this._CHART_PADDING_HORIZONTAL);
        this._appendAxis(svg, xScale, yScale, chartHeight);
        this._appendScatterDots(svg, xScale, yScale, dataSet)
    }

    _removeOld(selector) {
        d3.select(selector).select("svg").remove();
    }

    _createSvg(selector, width, height, paddingHorizontal) {
        return d3.select(selector)
            .append("svg")
            .attr("width", width)
            .attr("height", height)
            .attr("class", "scatter-chart")
            .append("g")
            .attr("transform", "translate(" + paddingHorizontal / 2 + "," + 0 / 2 + ")");
    }

    _createXScale(width, data) {
        return d3.scaleLinear()
            .range([0, width])
            .domain(d3.extent(data, datum => datum.x ))
    }

    _createYScale(height, data) {
        return d3.scaleLinear()
            .range([height, 0])
            .domain(d3.extent(data, datum => datum.y ))
    }

    _appendAxis(svg, xScale, yScale, height) {
        svg.append("g")
            .attr("transform", "translate(0," + height + ")")
            .call(d3.axisBottom(xScale));

        svg.append("g")
            .call(d3.axisLeft(yScale));
    }

    _appendScatterDots(svg, xScale, yScale, data) {
        svg.append('g')
            .selectAll("dot")
            .data(data)
            .enter()
                .append("circle")
                .attr("cx", d => xScale(d.x))
                .attr("cy", d => yScale(d.y))
                .attr("r", 3)
                .style("fill", this._SCATTER_COLOR)
    }


}