let Socneto = {

    Charts: {
        createLineChart: function (selector, dataSets, dataLabels) {
            let lineChart = new LineChart();
            lineChart.create(selector, dataSets, dataLabels);
        },

        createPieChart: function (selector, dataSets, dataLabels) {
            let pieChart = new PieChart();
            pieChart.create(selector, dataSets, dataLabels);
        },

        createScatterChart: function (selector) {
            let scatterChart = new ScatterChart();
            scatterChart.create(selector);
        }
    }

};

window.Socneto = Socneto;
