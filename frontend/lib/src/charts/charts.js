let Socneto = {

    Charts: {
        createLineChart: function (selector, datasets, datalabels) {
            let lineChart = new LineChart();
            lineChart.create(selector, datasets, datalabels);
        },
    }

};

window.Socneto = Socneto;
