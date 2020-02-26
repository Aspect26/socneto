let Socneto = {

    Charts: {
        createLineChart: function (selector, dataSets, dataLabels, isXDateTime, xAxisLabel) {
            let lineChart = new LineChart();
            lineChart.create(selector, dataSets, dataLabels, isXDateTime, xAxisLabel);
        },

        createPieChart: function (selector, dataSets, dataLabels) {
            let pieChart = new PieChart();
            pieChart.create(selector, dataSets, dataLabels);
        },

        createBarChart: function (selector, dataSets, dataLabels) {
            let barChart = new BarChart();
            barChart.create(selector, dataSets, dataLabels);
        },

        createTableChart: function (selector, dataSets, dataLabels) {
            let tableChart = new TableChart();
            tableChart.create(selector, dataSets, dataLabels);
        },

        createWordCloudChart: function (selector, dataSets) {
            let wordCloudChart = new WordCloudChart();
            wordCloudChart.create(selector, dataSets);
        },

        createScatterChart: function (selector, dataSet) {
            let scatterChart = new ScatterChart();
            scatterChart.create(selector, dataSet);
        }
    }

};

window.Socneto = Socneto;
