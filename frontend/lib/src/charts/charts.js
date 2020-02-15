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

        createScatterChart: function (selector, dataSet) {
            let scatterChart = new ScatterChart();
            scatterChart.create(selector, dataSet);
        }
    }

};

window.Socneto = Socneto;
