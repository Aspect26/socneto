class ScatterChart {

    _SCATTER_COLOR = "#396AB1";

    _ELEMENT_HEIGHT = 450;
    _CHART_PADDING_VERTICAL = 20;
    _CHART_PADDING_HORIZONTAL = 50;

    create(selector) {
        this._removeOld(selector);

        let data = [
            { x: 2009, y: 7.25 },
            { x: 2008, y: 6.55 },
            { x: 2007, y: 5.85 },
            { x: 2007, y: 5.8 },
            { x: 2006, y: 5.8 },
            { x: 2005, y: 5.6 },
            { x: 2004, y: 5.7 },
            { x: 2003, y: 5.2 },
            { x: 2002, y: 5.0 },
            { x: 2001, y: 5.5 },
            { x: 2000, y: 5.6 },
            { x: 1999, y: 5.32 },
            { x: 1998, y: 5.22 },
            { x: 1997, y: 5.15 },
            { x: 1996, y: 4.75 },
            { x: 1991, y: 4.25 },
            { x: 1981, y: 3.35 },
            { x: 1980, y: 3.10 },
            { x: 1979, y: 2.90 },
            { x: 1978, y: 2.65 }
        ];

        let elementWidth = document.getElementsByClassName("tab-content")[0].clientWidth;
        let chartWidth = elementWidth - this._CHART_PADDING_HORIZONTAL;
        let chartHeight = this._ELEMENT_HEIGHT - this._CHART_PADDING_VERTICAL;

        let xScale = this._createXScale(chartWidth, data);
        let yScale = this._createYScale(chartHeight, data);

        let svg = this._createSvg(selector, chartWidth, this._ELEMENT_HEIGHT, this._CHART_PADDING_HORIZONTAL);
        this._appendAxis(svg, xScale, yScale, chartHeight);
        this._appendScatterDots(svg, xScale, yScale, data)
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