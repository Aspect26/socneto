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

    _ELEMENT_HEIGHT = 700;
    _CHART_PADDING_VERTICAL = 20;
    _CHART_PADDING_HORIZONTAL = 10;
    _LABELS_MIDDLE_OFFSET = 1.2;

    create(selector, dataSet) {
        this._removeOld(selector);

        let element = document.getElementsByClassName("tab-content")[0];
        if (element == null)
            return;

        let elementWidth = element.clientWidth;
        let chartWidth = elementWidth - this._CHART_PADDING_HORIZONTAL;
        let chartHeight = this._ELEMENT_HEIGHT - this._CHART_PADDING_VERTICAL;
        let radius = Math.min(chartWidth, chartHeight) / 2;
        let totalSum = Object.values(dataSet).reduce((t, n) => t + n, 0);

        let svg = this._createSvg(selector, chartWidth, this._ELEMENT_HEIGHT);
        let color = this._createColorScale(dataSet, this._PIE_COLORS);
        let pieData = this._createPieData(dataSet);
        let arc = this._createPieArc(radius);

        this._createPieSlices(svg, pieData, arc, color);
        this._createPieLabels(svg, pieData, totalSum, arc, this._LABELS_MIDDLE_OFFSET);
    }

    _removeOld(selector) {
        d3.select(selector).selectAll("*").remove();
        d3.select(selector).html("");
    }

    _createSvg(selector, width, height) {
        return d3.select(selector)
            .append("svg")
            .attr("width", "100%")
            .attr("height", height)
            .attr("class", "pie-chart")
            .append("g")
            .attr("transform", "translate(" + width / 2 + "," + height / 2 + ")");
    }

    _createColorScale(dataSet, colors) {
        return d3.scaleOrdinal()
            .domain(dataSet)
            .range(colors);
    }

    _createPieData(dataSet) {
        let pie = d3.pie()
            .value(d => d.value);

        return pie(d3.entries(dataSet));
    }

    _createPieArc(radius) {
        return d3.arc()
            .innerRadius(radius)
            .outerRadius(radius / 5);
    }

    _createPieSlices(svg, pieData, pieArc, colorScale) {
        svg.append('g').attr('class', 'slices');

        let slice = svg.select(".slices").selectAll("path.slice").data(pieData);
        slice.enter()
            .insert("path")
            .style("fill", d => colorScale(d.data.key))
            .style("stroke", "white")
            .attr("class", "slice")
            .attr('d', pieArc)
            .exit()
            .remove();

        let slices = svg.select(".slices").selectAll("path.slice");
        slices
            .on('mouseover', (_, hoveredElementIndex) => {
                slices.transition().style("opacity", 0.5);
                d3.select(slices.nodes()[hoveredElementIndex]).transition().style("opacity", 1);
            })
            .on('mouseout', (d, i) => {
                slices.transition().style("opacity", 1.0);
            });
    }

    _createPieLabels(svg, pieData, dataTotalSum, pieArc, textOffsetMiddle) {
        svg.append("g").attr("class", 'labels');
        svg.select(".labels").attr("text-anchor", "middle");

        let label = svg.select(".labels").selectAll("text").data(pieData);
        label.enter()
            .append("text")
            .attr("dy", ".35em")
            .attr("transform", d => {
                let pos = pieArc.centroid(d).map(d => d * textOffsetMiddle);
                return "translate("+ pos +")";
            });

        svg.select(".labels").selectAll("text").insert("tspan")
            .text(d => d.data.key)
            .attr("class", "label")
            .attr("y", "-0.7em");

        svg.select(".labels").selectAll("text").insert("tspan")
            .text(d => ((d.data.value / dataTotalSum) * 100).toFixed(1) + "% (" + d.data.value + ")")
            .attr("class", "value")
            .attr("x", 0)
            .attr("y", "0.7em")
    }

}