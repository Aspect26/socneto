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

function createLineChart(selector, datasets, datalabels) {

    d3.select(selector).select("svg").remove();

    var margin = {top: 50, right: 50, bottom: 50, left: 50};
    var legendWidth = 200;

    w = document.getElementsByClassName("tab-content")[0].clientWidth;
    h = 450;

    var chartWidth = w - margin.left - margin.right - legendWidth;
    var chartHeight = h - margin.top - margin.bottom;


    var xScale = d3.scaleTime()
        .domain(d3.extent(datasets.flat(), function(d) { return new Date(d.date); }))
        .range([0, chartWidth]);

    var yScale = d3.scaleLinear()
        .domain([0, 1])
        .range([chartHeight, 0]);

    var line = d3.line()
        .x(function (d, i) {
            return xScale(new Date(d.date));
        })
        .y(function (d) {
            return yScale(d.y);
        })
        .curve(d3.curveMonotoneX);

    var svg = d3.select(selector)
        .append("svg")
        .attr("width", w)
        .attr("height", h)
        .append("g")
        .attr("transform", "translate(" + margin.left  + "," + margin.top + ")");

    svg.append("g")
        .attr("class", "x axis")
        .attr("transform", "translate(" + legendWidth + "," + chartHeight + ")")
        .call(d3.axisBottom(xScale));

    svg.append("g")
        .attr("class", "y axis")
        .attr("transform", "translate(" + legendWidth + ", 0)")
        .call(d3.axisLeft(yScale));

    for (var i = 0; i < datasets.length; ++i) {
        var currentColor = _LINE_COLORS[i % _LINE_COLORS.length];
        var currentTitle = datalabels[i];

        svg.append("path")
            .datum(datasets[i])
            .attr("class", "line")
            .style('stroke', currentColor)
            .attr("transform", "translate(" + legendWidth + ", 0)")
            .attr("d", line);

        svg.append("circle").attr("cx", 0).attr("cy", i * 20).attr("r", 6).style("fill", currentColor);
        svg.append("text").attr("x", 20).attr("y", i * 20).text(currentTitle).style("font-size", "15px").attr("alignment-baseline","middle")
    }

}