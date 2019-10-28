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

    create(selector, dataSets, dataLabels) {
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

        let data = {a: 9, b: 20, c: 30, d: 8, e: 12};
        let totalSum = Object.values(data).reduce((t, n) => t + n);

        let color = d3.scaleOrdinal()
            .domain(data)
            .range(this._PIE_COLORS);

        let pie = d3.pie()
            .value(d => d.value);

        let data_ready = pie(d3.entries(data));

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
            .text(d => (d.data.value / totalSum).toFixed(2) + "% (" + d.data.value + ")")
            .attr("class", "value")
            .attr("x", 0)
            .attr("y", "0.7em")
    }

    _removeOld(selector) {
        d3.select(selector).select("svg").remove();
    }

}