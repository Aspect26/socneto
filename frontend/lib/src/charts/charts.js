let Socneto = {

    Charts: {
        createLineChart: function (selector, dataSets, dataLabels) {
            let lineChart = new LineChart();
            lineChart.create(selector, dataSets, dataLabels);
        },

        createPieChart: function (selector, dataSets, dataLabels) {
            let pieChart = new PieChart();
            pieChart.create(selector, dataSets, dataLabels);
        }
    }

};

window.Socneto = Socneto;
