class TableChart {

    _CHART_PADDING_VERTICAL = 30;
    _CHART_PADDING_HORIZONTAL = 30;

    create(selector, dataSet, label) {
        this._removeOld(selector);

        let element = document.getElementsByClassName("tab-content")[0];
        if (element == null)
            return;

        let tableData = this._createTableData(dataSet);
        label = label != null && label !== ""? label : "name";

        let elementWidth = element.clientWidth;
        let chartWidth = elementWidth - this._CHART_PADDING_HORIZONTAL * 2;

        let paddingLeft = this._CHART_PADDING_HORIZONTAL;

        let table = this._createTable(selector, chartWidth, paddingLeft, this._CHART_PADDING_VERTICAL);
        this._createHeader(table, label);
        this._createRows(table, tableData);
    }

    _removeOld(selector) {
        d3.select(selector).selectAll("*").remove();
        d3.select(selector).html("");
    }

    _createTableData(dataSet) {
        let data = [];
        Object.keys(dataSet).forEach((key, index, map) => {
            data.push({'name': key, 'value': dataSet[key]});
        });

        data.sort((a ,b) => b.value - a.value);

        let totalSum = data.reduce((acc, item) => acc + item.value, 0);
        for (let i = 0; i < data.length; ++i) {
            let itemPercentage = data[i].value / totalSum;
            data[i].key = `${data[i].key} (${(itemPercentage * 100).toFixed(1)}%)`;
        }

        return data;
    }

    _createTable(selector, width, paddingLeft, paddingTop) {
        return d3.select(selector)
            .append("table")
            .attr("class", "table-chart")
            .attr("transform", "translate(" + paddingLeft + "," + paddingTop + ")");
    }

    _createHeader(table, label) {
        let columns = ["name", "value"];

        let thead = table.append("thead");
        thead.append("tr")
            .selectAll("th")
            .data(columns)
            .enter()
            .append("th")
            .text((column, i) => i % 2 === 0? label : "value");
    }

    _createRows(table, tableData) {
        let tbody = table.append("tbody");
        let rows = tbody.selectAll("tr")
            .data(tableData)
            .enter()
            .append("tr")
            .attr("class", (d, i) => i % 2 === 0? "highlight": "no-highlight");

        let columns = ["name", "value"];
        rows.selectAll("td")
            .data((row) => {
                return columns.map((column) => {return {
                    name: column,
                    value: row[column]
                }})
            })
            .enter()
            .append("td")
            .html((d) => d.value);
    }

}
