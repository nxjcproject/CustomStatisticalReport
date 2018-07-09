$(function () {
    InitDate();
    loadDataGrid("first");
});

//初始化日期框
function InitDate() {
    var nowDate = new Date();
    var beforeDate = new Date();
    nowDate.setDate(nowDate.getDate() - 1);
    beforeDate.setDate(nowDate.getDate() - 10);
    var nowString = nowDate.getFullYear() + '-' + (nowDate.getMonth() + 1) + '-' + nowDate.getDate();
    var beforeString = beforeDate.getFullYear() + '-' + (beforeDate.getMonth() + 1) + '-' + beforeDate.getDate();
    $('#startTime').datebox('setValue', beforeString);
    $('#endTime').datebox('setValue', nowString);
}

function loadDataGrid(type, myData) {
    if (type == "first") {
        $('#grid_ComprehensiveConsumption').datagrid({
            columns: [[
                    { field: 'Name', title: '分厂', width: 100 },
                    { field: 'clinkerConsumption', title: '熟料综合电耗', width: 100, align: 'right'},
                    { field: 'cementConsumption', title: '水泥综合电耗', width: 100, align: 'right'}
            ]],
            fit: true,
            toolbar: "#toolbar_ComprehensiveConsumption",
            rownumbers: true,
            singleSelect: true,
            striped: true,
            data: []
        })
    }
    else {
        $('#grid_ComprehensiveConsumption').datagrid("loadData", myData);
    }
}

function Query() {
    var startTime = $('#startTime').datetimebox('getValue');//开始时间
    var endTime = $('#endTime').datetimebox('getValue');//结束时间
    var win = $.messager.progress({
        title: '请稍后',
        msg: '数据载入中...'
    });
    $.ajax({
        type: "POST",
        url: "report_ComprehensiveConsumptionStatistics.aspx/GetComprehensiveConsumptionData",
        data: "{mStartTime:'" + startTime + "',mEndTime:'" + endTime + "'}",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (msg) {
            $.messager.progress('close');
            var myData = jQuery.parseJSON(msg.d);
            if (myData != undefined && myData.length == 0) {
                loadDataGrid("last", []);
                $.messager.alert('提示', '没有查询到记录！');
            } else {
                $('#grid_ComprehensiveConsumption').datagrid("loadData", myData);
            }
        },
        beforeSend: function (XMLHttpRequest) {
            win;
        },
        error: function () {
            $.messager.progress('close');
            $("#grid_ComprehensiveConsumption").datagrid('loadData', []);
            $.messager.alert('失败', '加载失败！');
        }
    })
}