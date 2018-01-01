var SelectOrganizationName = "";
var SelectDatetime = "";
$(function () {
    SetDateboxValue();
    LoadTreeGrid({ "rows": [], "total": 0 });
});
// datetime datebox
function SetDateboxValue() {
    var m_Oneday = 1000 * 60 * 60 * 24;
    var m_CurrentDate = new Date();
    var m_Yesterday = new Date(m_CurrentDate - m_Oneday);

    var m_StrDate = m_Yesterday.getFullYear() + "-";
    m_StrDate += m_Yesterday.getMonth() + 1 + "-";
    m_StrDate += m_Yesterday.getDate();
    //strDate += curr_time.getHours()+":";
    //strDate += curr_time.getMinutes()+":";
    //strDate += curr_time.getSeconds();
    $("#datetime").datebox("setValue", m_StrDate);
}
function onOrganisationTreeClick(node) {
    $('#TextBox_OrganizationName').textbox('setText', node.text);
    $('#organizationId').val(node.OrganizationId);
}
function QueryReportFun() {
    var m_OrganizationId = $('#organizationId').val();
    SelectOrganizationName = $('#TextBox_OrganizationName').textbox('getText');
    SelectDatetime = $("#datetime").datebox("getValue");
    if (m_OrganizationId != undefined && m_OrganizationId != "" && SelectDatetime != undefined && SelectDatetime != "") {
        var win = $.messager.progress({
            title: '请稍后',
            msg: '数据载入中...'
        });
        $.ajax({
            type: "POST",
            url: "DayConsumptionReport.aspx/GetReportData",
            data: '{organizationId: "' + m_OrganizationId + '", datetime: "' + SelectDatetime + '"}',
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: function (msg) {
                $.messager.progress('close');
                var m_MsgData = jQuery.parseJSON(msg.d);
                //$.each(m_Data, function (i, item) {
                //    var value = Number(item.Value)
                //    var element = $(document.getElementById(item.ID));
                //    //if (element.attr("tagName") == "span")
                //    //element.html(value.toFixed(0));
                //    element.html(value);
                //    //else
                //    //element.val(value.toFixed(0));
                //    //$('#zc_nxjc_byc_byf_clinker01_limestoneMine_ElectricityQuantity').html(m_Data.Value);
                //});
                $('#TreeGrid_ReportTable').treegrid("loadData", m_MsgData);
                $('#TreeGrid_ReportTable').treegrid("collapseAll");
            }
        });
    }
    else {
        alert("您没有选择分厂或者未选择时间!");
    }

}
function LoadTreeGrid(myData) {
    try {
        $('#TreeGrid_ReportTable').treegrid({
            data: myData,
            dataType: "json",
            //loadMsg: '',   //设置本身的提示消息为空 则就不会提示了的。这个设置很关键的
            idField: 'id',
            treeField: 'Name',
            rownumbers: true,
            singleSelect: true,
            frozenColumns: [[{
                width: '220',
                title: '区域及工序',
                field: 'Name'
            }
            ]],
            columns: [[{
                width: '100',
                title: '变量ID',
                field: 'VariableId',
                hidden: true
            },{
                width: '100',
                title: '组织机构层次码',
                field: 'OrganizationLevelCode',
                hidden: true
            }, {
                width: '100',
                title: '层次码',
                field: 'LevelCode',
                hidden: true
            }, {
                width: '100',
                title: '用电量',
                field: 'DayElectricityQuantity'
            }, {
                width: '100',
                title: '月累计用电量',
                field: 'TotalElectricityQuantity'
            }, {
                width: '100',
                title: '分母',
                field: 'Denominator',
                hidden: true
            }, {
                width: '80',
                title: '名称',
                field: 'MaterialName'
            }, {
                width: '80',
                title: '生产量',
                field: 'DayOutput'
            }, {
                width: '100',
                title: '月累计生产量',
                field: 'TotalOutput'
            }, {
                width: '80',
                title: '电耗',
                field: 'DayElectricityConsumption'
            }, {
                width: '80',
                title: '月累计电耗',
                field: 'TotalElectricityConsumption'
            }]],
            toolbar: '#toolbar_ReportTable'
        });

    }
    catch (e) {

    }
}
function RefreshFun() {
    QueryReportFun();
}
function ExportFileFun() {
    var m_FunctionName = "ExcelStream";
    var m_Parameter1 = GetTreeTableHtml("TreeGrid_ReportTable", "能耗日报", "Name", SelectOrganizationName, SelectDatetime);
    var m_Parameter2 = SelectOrganizationName;

    var m_ReplaceAlllt = new RegExp("<", "g");
    var m_ReplaceAllgt = new RegExp(">", "g");
    m_Parameter1 = m_Parameter1.replace(m_ReplaceAlllt, "&lt;");
    m_Parameter1 = m_Parameter1.replace(m_ReplaceAllgt, "&gt;");

    var form = $("<form id = 'ExportFile'>");   //定义一个form表单
    form.attr('style', 'display:none');   //在form表单中添加查询参数
    form.attr('target', '');
    form.attr('method', 'post');
    form.attr('action', "DayConsumptionReport.aspx");

    var input_Method = $('<input>');
    input_Method.attr('type', 'hidden');
    input_Method.attr('name', 'myFunctionName');
    input_Method.attr('value', m_FunctionName);
    var input_Data1 = $('<input>');
    input_Data1.attr('type', 'hidden');
    input_Data1.attr('name', 'myParameter1');
    input_Data1.attr('value', m_Parameter1);
    var input_Data2 = $('<input>');
    input_Data2.attr('type', 'hidden');
    input_Data2.attr('name', 'myParameter2');
    input_Data2.attr('value', m_Parameter2);

    $('body').append(form);  //将表单放置在web中 
    form.append(input_Method);   //将查询参数控件提交到表单上
    form.append(input_Data1);   //将查询参数控件提交到表单上
    form.append(input_Data2);   //将查询参数控件提交到表单上
    form.submit();
    //释放生成的资源
    form.remove();
}
function PrintFileFun() {
    var m_ReportTableHtml = GetTreeTableHtml("TreeGrid_ReportTable", "能耗日报", "Name", SelectOrganizationName, SelectDatetime);
    PrintHtml(m_ReportTableHtml);
}
