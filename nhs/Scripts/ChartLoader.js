var assignedBarChartOptions = {
    series: [],
    chart: {
        height: 350,
        type: 'radialBar',
        toolbar: {
            show: false
        }
    },
    plotOptions: {
        radialBar: {
            hollow: {
                size: '70%',
            }
        },
    },
    labels: ['Assigned'],
    noData: {
        text: 'loading...'
    }
};

// Assigned radial bar chart Ends

var userChartOptions = {
    title: {
        // text: 'Users',
        floating: true,
        offsetY: 0,
        align: 'center',
        style: {
            color: '#444'
        }
    },
    series: [],
    chart: {
        type: 'bar',
        height: 350,
        stacked: true,
        toolbar: {
            show: false
        },
        zoom: {
            enabled: true
        },
        events: {
            legendClick: function (chartContext, seriesIndex, config) {
                alert('Legend clicked event fired');
            }
        }
    },
    noData: {
        text: 'Loading...'
    },
    responsive: [
        {
            breakpoint: 480,
            options: {
                legend: {
                    position: 'bottom',
                    offsetX: -10,
                    offsetY: 0
                }
            }
        }
    ],
    plotOptions: {
        bar: {
            horizontal: false,
        },
    },
    xaxis: {
        type: '',
        categories: [],
        axisBorder: {
            show: true,
        },
    },
    grid: {
        show: false,
    },
    yaxis: {
        labels: {
            show: false
        },
        axisBorder: {
            show: true,
        }
    },
    legend: {
        position: 'right',
        offsetY: 40
    },
    fill: {
        opacity: 1
    }
};

// User chart Ends

function minTommss(minutes) {
    var sign = minutes < 0 ? "-" : "";
    var min = Math.floor(Math.abs(minutes));
    var sec = Math.floor((Math.abs(minutes) * 60) % 60);
    return sign + (min < 10 ? "0" : "") + min + ":" + (sec < 10 ? "0" : "") + sec;
}

// m:ss to minute decimal
function minDecimal(min, sec) {
    return (min + sec * 0.0168).toFixed(2);
}

var avgTimeByModuleChartOptions = {
    series: [],
    grid: {
        show: false,
    },
    chart: {
        height: 350,
        type: 'bar',
        toolbar: {
            show: false
        },
    },
    noData: {
        text: 'Loading...'
    },
    plotOptions: {
        bar: {
            dataLabels: {
                position: 'top', // top, center, bottom
            },
        }
    },
    dataLabels: {
        enabled: true,
        formatter: function (val) {
            return minTommss(val);
        },
        offsetY: -20,
        style: {
            fontSize: '12px',
            colors: ["#304758"]
        }
    },
    xaxis: {
        type: '',
        categories: [],
        position: 'bottom',
        axisBorder: {
            show: true
        },
        axisTicks: {
            show: false
        },
        crosshairs: {
            fill: {
                type: 'gradient',
                gradient: {
                    colorFrom: '#D8E3F0',
                    colorTo: '#BED1E6',
                    stops: [0, 100],
                    opacityFrom: 0.4,
                    opacityTo: 0.5,
                }
            }
        },
        tooltip: {
            enabled: true,
        }
    },
    yaxis: {
        labels: {
            show: false,
            //formatter: function (value) {
            //    return minTommss(value);
            //}
        },
        axisBorder: {
            show: true,
        }
    },
    title: {
        //text: 'AVG Time - Module',
        //floating: true,
        offsetY: 0,
        align: 'center',
        style: {
            color: '#444'
        }
    }
};

// End AVG Time - Module

var sessionAvgTimeByUserByModuleOpt = {
    title: {
        // text: 'Sessions (AVG) by User by Module',
        floating: true,
        offsetY: 0,
        align: 'center',
        style: {
            color: '#444'
        }
    },
    grid: {
        show: false,
    },
    series: [],
    chart: {
        type: 'bar',
        height: 350,
        stacked: true,
        toolbar: {
            show: false
        },
        zoom: {
            enabled: true
        }
    },
    noData: {
        text: 'Loading...'
    },
    responsive: [
        {
            breakpoint: 480,
            options: {
                legend: {
                    position: 'bottom',
                    offsetX: -10,
                    offsetY: 0
                }
            }
        }
    ],
    plotOptions: {
        bar: {
            horizontal: false,
        },
    },
    xaxis: {
        type: '',
        categories: [],
        axisBorder: {
            show: true,
        },
    },
    yaxis: {
        labels: {
            show: false
        },
        axisBorder: {
            show: true,
        }
    },
    legend: {
        position: 'right',
        offsetY: 40
    },
    fill: {
        opacity: 1
    }
};

// End Session AVG Time By user By Module

var userOverTimeOpt = {
    title: {
        //text: 'Users Over Time',
        floating: true,
        offsetY: 0,
        align: 'center',
        style: {
            color: '#444'
        }
    },
    grid: {
        show: false,
    },
    series: [],
    chart: {
        type: 'line',
        height: 350,
        stacked: true,
        toolbar: {
            show: false
        },
        events: {
            legendClick: function (chartContext, seriesIndex, config) {
                alert('Legend clicked event fired');
            }
        }
    },
    noData: {
        text: "Loading..."
    },
    dataLabels: {
        enabled: false
    },
    stroke: {
        curve: 'smooth'
    },
    xaxis: {
        // title: 'Day',
        type: '',
        categories: [],
        axisBorder: {
            show: true,

        },
    },
    yaxis: {
        axisBorder: {
            show: true,
        }
    }

};


// End User Over time Chart
var avgTimeSpentModuleOpt = {
    title: {
        //text: 'AVG Time Spent Module',
        floating: true,
        offsetY: 0,
        align: 'center',
        style: {
            color: '#444'
        }
    },
    grid: {
        show: false,
    },
    series: [],
    chart: {
        type: 'line',
        height: 350,
        stacked: true,
        toolbar: {
            show: false
        },
    },
    noData: {
        text: 'Loading...'
    },
    dataLabels: {
        enabled: false
    },
    stroke: {
        curve: 'smooth'
    },
    xaxis: {
        // title: 'Day',
        type: '',
        categories: [],
        axisBorder: {
            show: true,

        },
    },
    yaxis: {
        axisBorder: {
            show: true,

        },
    }

};


// End AVG Time spent by Module
var sessionChartOpt = {
    series: [],
    grid: {
        show: false,
    },
    chart: {
        type: 'bar',
        height: 350,
        toolbar: {
            show: false
        },
    },
    noDate: {
        text: 'Loading...'
    },
    plotOptions: {
        bar: {
            horizontal: true,
            dataLabels: {
                position: 'top',
            },
        }
    },
    dataLabels: {
        enabled: true,
        offsetX: -6,
        style: {
            fontSize: '12px',
            colors: ['#fff']
        }
    },
    xaxis: {
        categories: [],
        axisBorder: {
            show: true,

        },
    },
    yaxis: {
        axisBorder: {
            show: true,

        },
    }
};

// End of session chart

var sessionTimeChartOpt = {
    series: [],
    chart: {
        height: 350,
        type: 'line',
        zoom: {
            enabled: false
        },
        toolbar: {
            show: false
        }
    },
    noData: {
        text: 'Loading...'
    },
    dataLabels: {
        enabled: false
    },
    stroke: {
        curve: 'straight'
    },
    grid: {
        show: false
    },
    xaxis: {
        categories: []
    }
};

function LoadAllChart() {
    var assignedRadialBarChart = new ApexCharts(document.querySelector("#assignedBarChart"), assignedBarChartOptions);
    assignedRadialBarChart.render();

    $.getJSON('/Home/AssignedLicenseBarChartResult', function (response) {
        assignedRadialBarChart.updateSeries([response]);
    });


    var userChart = new ApexCharts(document.querySelector("#userChart"), userChartOptions);
    userChart.render();

    $.getJSON('/Home/UserLicenseChartResult', function (response) {

        $('#lblNoOfLicenseAllocated').html(response.LicenseInfo.TotalNodLicenseAllocated);
        $('#lblNoOfUnusedLicense').html(response.LicenseInfo.NoOfUnusedLicense);
        $('#lblNofUsedLicense').html(response.LicenseInfo.NoOfUsedLicense);

        userChart.updateOptions({
            xaxis: {
                type: response.ChartOptions.X_axisCategoryType,
                categories: response.ChartOptions.X_axisCategories,
            }
        });

        userChart.updateSeries(response.ChartInfo);
    });


    var avgTimeChartByModule =
        new ApexCharts(document.querySelector("#avgTimeChartByModule"), avgTimeByModuleChartOptions);
    avgTimeChartByModule.render();

    $.getJSON('/Home/AvgTimeModuleChartResult', function (response) {

        avgTimeChartByModule.updateSeries(response.ChartInfo);
        avgTimeChartByModule.updateOptions({
            xaxis: {
                type: response.ChartOptions.X_axisCategoryType,
                categories: response.ChartOptions.X_axisCategories,

            },
            yaxis: {
                labels: {
                    show: false,
                    formatter: function (value) {
                        return minTommss(value);
                    }
                },
                axisBorder: {
                    show: true,
                }
            },
        });
    });

    var sessionAvgTimeByUserByModuleChart = new ApexCharts(document.querySelector("#sessionSvgByUserByModule"),
        sessionAvgTimeByUserByModuleOpt);
    sessionAvgTimeByUserByModuleChart.render();

    $.getJSON('/Home/SessionAvgTimeByUserByModuleChartResult', function (response) {

        sessionAvgTimeByUserByModuleChart.updateOptions({
            xaxis: {
                type: response.ChartOptions.X_axisCategoryType,
                categories: response.ChartOptions.X_axisCategories,
            }
        });

        sessionAvgTimeByUserByModuleChart.updateSeries(response.ChartInfo);
    });


    var userOverTimeChart = new ApexCharts(document.querySelector("#usersOverTime"), userOverTimeOpt);
    userOverTimeChart.render();

    $.getJSON('/Home/UserOverTimeChartResult', function (response) {

        userOverTimeChart.updateOptions({
            xaxis: {
                type: response.ChartOptions.X_axisCategoryType,
                categories: response.ChartOptions.X_axisCategories,
            }
        });

        userOverTimeChart.updateSeries(response.ChartInfo);
    });

    var avgTimeSpentModuleChart =
        new ApexCharts(document.querySelector("#avgTimeSpentModule"), avgTimeSpentModuleOpt);
    avgTimeSpentModuleChart.render();

    $.getJSON('/Home/AvgSpentTimeByModuleChartResult', function (response) {

        avgTimeSpentModuleChart.updateOptions({
            xaxis: {
                type: response.ChartOptions.X_axisCategoryType,
                categories: response.ChartOptions.X_axisCategories,
            }
        });

        avgTimeSpentModuleChart.updateSeries(response.ChartInfo);
    });

    var sessionChart = new ApexCharts(document.querySelector("#sessionChart"), sessionChartOpt);
    sessionChart.render();

    $.getJSON('/Home/SessionChartResult', function (response) {

        sessionChart.updateOptions({
            xaxis: {
                type: response.ChartOptions.X_axisCategoryType,
                categories: response.ChartOptions.X_axisCategories,
            }
        });

        sessionChart.updateSeries(response.ChartInfo);
    });


    var sessionTimeChart = new ApexCharts(document.querySelector("#sessionTimeChart"), sessionTimeChartOpt);
    sessionTimeChart.render();

    $.getJSON('/Home/SessionTimeChartResult', function (response) {

        sessionTimeChart.updateOptions({
            xaxis: {
                type: response.ChartOptions.X_axisCategoryType,
                categories: response.ChartOptions.X_axisCategories,
            }
        });

        sessionTimeChart.updateSeries(response.ChartInfo);
    });
}
