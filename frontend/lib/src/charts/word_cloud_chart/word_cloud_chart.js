class WordCloudChart {

    _WORD_COLORS = [
        "#396AB1",
        "#DA7C30",
        "#3E9651",
        "#CC2529",
        "#535154",
        "#6B4C9A",
        "#922428",
        "#948B3D"
    ];

    _CHART_PADDING_VERTICAL = 30;
    _CHART_PADDING_HORIZONTAL = 30;

    _CHART_HEIGHT = 500;

    _MAX_FONT_SIZE = 50;
    _MIN_FONT_SIZE = 15;

    create(selector, dataSet) {
        this._removeOld(selector);

        let element = document.getElementsByClassName("tab-content")[0];
        if (element == null)
            return;

        let data = this._createWordCloudData(dataSet);

        let elementWidth = element.clientWidth;
        let chartWidth = elementWidth - this._CHART_PADDING_HORIZONTAL * 2;

        let svg = this._createSvg(selector, chartWidth, this._CHART_HEIGHT, this._CHART_PADDING_HORIZONTAL, this._CHART_PADDING_VERTICAL);
        let layout = this._createWordCloud(svg, chartWidth, this._CHART_HEIGHT, data);
        this._drawWordCloud(svg, layout)
    }

    _removeOld(selector) {
        d3.select(selector).selectAll("*").remove();
        d3.select(selector).html("");
    }

    _createWordCloudData(dataSet) {
        let data = [];
        let maxValue = Object.keys(dataSet).reduce((acc, key) => dataSet[key] > acc? dataSet[key] : acc , 0);

        Object.keys(dataSet).forEach((key, index, map) => {
            let word = key;
            let amount = dataSet[key];
            let ratioToMax = amount / maxValue;
            let fontSize = Math.round((this._MAX_FONT_SIZE - this._MIN_FONT_SIZE) * ratioToMax + this._MIN_FONT_SIZE);

            data.push({'word': word, 'amount': fontSize});
        });

        data.sort((a ,b) => b.value - a.value);
        return data;
    }

    _createSvg(selector, width, height, paddingHorizontal, paddingVertical) {
        return d3.select(selector)
            .append("svg")
            .attr("class", "word-cloud-chart")
            .attr("width", "100%")
            .attr("height", height + 2 * paddingVertical)
            .append("g")
                .attr("transform", "translate(" + paddingHorizontal + "," + paddingVertical + ")");
    }

    _createWordCloud(svg, width, height, data) {
        return d3.layout.cloud()
            .size([width, height])
            .words(data.map(d => { return { text: d.word, size: d.amount } }))
            .padding(5)
            .rotate((d) => ~~(Random.forWord(d.text) * 2) * 90)
            .fontSize(d => d.size)
    }

    _drawWordCloud(svg, layout) {
        let colors = this._WORD_COLORS;

        layout.on("end", data => {
            svg.append("g")
                .attr("transform", "translate(" + layout.size()[0] / 2 + "," + layout.size()[1] / 2 + ")")
                .selectAll("text")
                .data(data)
                .enter().append("text")
                .style("font-size", d => `${d.size}px`)
                .style("fill", (d, i) => colors[i % colors.length])
                .attr("text-anchor", "middle")
                .style("font-family", "Impact")
                .attr("transform", d => "translate(" + [d.x, d.y] + ") rotate(" + d.rotate + ")")
                .text(d => d.text)
        });

        layout.start();
    }

}

class Random {}
Random.forWord = function (word) {
    let wordHash = word.split('').reduce((a,b) => { a = ((a << 5) - a) + b.charCodeAt(0); return a & a }, 0);
    return Math.sin(wordHash) / 2 + 0.5;
};
