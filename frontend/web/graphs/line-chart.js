function createLineChart(selector, dataset, w, h) {

    var margin = {top: 50, right: 50, bottom: 50, left: 50};
    var width = w - margin.left - margin.right;
    var height = h - margin.top - margin.bottom;

    var n = dataset.length;

    var xScale = d3.scaleTime()
        .domain(d3.extent(dataset, function(d) { return new Date(d.date); }))
        .range([0, width]);

    var yScale = d3.scaleLinear()
        .domain([0, 1])
        .range([height, 0]);

    var line = d3.line()
        .x(function (d, i) {
            return xScale(new Date(d.date));
        })
        .y(function (d) {
            return yScale(d.y);
        })
        .curve(d3.curveMonotoneX);


    var svg = d3.select(selector).append("svg")
        .attr("width", width + margin.left + margin.right)
        .attr("height", height + margin.top + margin.bottom)
        .append("g")
        .attr("transform", "translate(" + margin.left + "," + margin.top + ")");

    svg.append("g")
        .attr("class", "x axis")
        .attr("transform", "translate(0," + height + ")")
        .call(d3.axisBottom(xScale));

    svg.append("g")
        .attr("class", "y axis")
        .call(d3.axisLeft(yScale));

    svg.append("path")
        .datum(dataset)
        .attr("class", "line")
        .attr("d", line);

    svg.selectAll(".dot")
        .data(dataset)
        .enter()
        .append("circle")
        .attr("class", "dot")
        .attr("cx", function(d, i) { return xScale(i) })
        .attr("cy", function(d) { return yScale(d.y) })
        .attr("r", 5)
          .on("mouseover", function(a, b, c) {
            //d3.select(this).attr('class', 'focus')
          })
          .on("mouseout", function() {
            //d3.select(this).attr('class', '')
          });
}