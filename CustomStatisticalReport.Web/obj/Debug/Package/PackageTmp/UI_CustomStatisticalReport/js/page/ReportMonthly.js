
var g_organizationId = '';

$(function () {
    InitDate();
    LoadHtml(g_templateURL);//g_templateURL在每个aspx中定义
});
//初始化日期框
function InitDate() {
    var nowDate = new Date();
    nowDate.setDate(nowDate.getDate() - 1);
    var dateString = nowDate.getFullYear() + '-' + (nowDate.getMonth());
    $('#datetime').datebox('setValue', dateString);
}

//加载报表模板
function LoadHtml(t_url) {
    //$("#contain").empty();
    $("#contain").load(t_url);
}

//目录树双击事件
function onOrganisationTreeClick(node) {
    $('#productLineName').textbox('setText', node.text);
    $('#organizationId').val(node.OrganizationId);
    g_organizationId = node.OrganizationId;

}

function QueryReportFun() {
    LoadHtml(g_templateURL);
    var dateTime = $("#datetime").datebox("getValue");
    $.ajax({
        type: "POST",
        url: "/UI_CustomStatisticalReport/ProductionReport/ProductionReportService.asmx/" + g_webmethod,
        data: '{organizationId: "' + g_organizationId + '", datetime: "' + dateTime + '"}',
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (msg) {
            FillCell(msg.d);
        },
        error: function () { $.messager.alert('失败', '获取数据失败'); }
    })
}
//填充单元格数据
function FillCell(myData) {
    for (var item in myData) {
        var m_obj = $(document.getElementById(item));//ID中有特殊字符，jquery不支持，先有js获得对象再转换成jquery对象      
        if (m_obj.length != 0) {
            var value = Number(myData[item]).toFixed(2) == "NaN" ? myData[item] : Number(myData[item]).toFixed(2);
            //m_obj.children().attr("value", value);//保留两位小数
            m_obj.html(value);//保留两位小数
        }
    }
}

//日期框格式控制
function myformatter(date) {
    var y = date.getFullYear();
    var m = date.getMonth() + 1;
    var d = date.getDate();
    return y + '-' + (m < 10 ? ('0' + m) : m);//+ '-' + (d < 10 ? ('0' + d) : d);
}
function myparser(s) {
    if (!s) return new Date();
    var ss = (s.split('-'));
    var y = parseInt(ss[0], 10);
    var m = parseInt(ss[1], 10);
    //var d = parseInt(ss[2], 10);
    if (!isNaN(y) && !isNaN(m)) {
        return new Date(y, m - 1, 1);
    } else {
        return new Date();
    }
}