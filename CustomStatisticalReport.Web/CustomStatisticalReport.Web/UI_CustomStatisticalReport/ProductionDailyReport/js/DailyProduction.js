$(function () {
    Loadtreegrid("first", { "rows": [], "total": 0 });
    InitDate();

});
//初始化日期框
function InitDate() {
    var nowDate = new Date();
    nowDate.setDate(nowDate.getDate() - 1);
    var dateString = nowDate.getFullYear() + '-' + (nowDate.getMonth() + 1) + '-' + (nowDate.getDate());
    $('#DatetimeF').datebox('setValue', dateString);
}
function onOrganisationTreeClick(myNode) {
    $('#OrganizationIdF').attr('value', myNode.OrganizationId);  //textbox('setText', myNode.OrganizationId);
    $('#OrganizationNameF').textbox('setText', myNode.text);
}
function Loadtreegrid(type, myData) {
    if (type == "first") {
        $('#DailyProduction').treegrid({
            idField: 'id',
            treeField: 'text',
            rownumbers: true,
            singleSelect: true,
            toolbar: '#toolbar_head',
            data: myData,
            fit: true,
            frozenColumns: [[
                { field: 'id', title: 'Id', width: 110, align: 'center', hidden:true },
                { field: 'text', title: '名称', width: 130, align: 'center' }
            ]],
            columns: [[
                 { title: '月计划', colspan: 4, width: 280, align: 'center' },
                 { title: '日计', colspan: 4, width: 280, align: 'center' },
                 { title: '月合计', colspan: 4, width: 280, align: 'center' },
                 { title: '年累计', colspan: 4, width: 280, align: 'center' }
            ], [
                { field: 'Output_Plan', title: '产量（t）', width: 70, align: 'center' },
                { field: 'RunTime_Plan', title: '运行时间(h)', width: 70, align: 'center' },
                { field: 'TimeOutput_Plan', title: '台时(t/h)', width: 70, align: 'center' },
                { field: 'RunRate_Plan', title: '运转率(%)', width: 70, align: 'center' },
                { field: 'Output_Day', title: '产量（t）', width: 70, align: 'center' },
                { field: 'RunTime_Day', title: '运行时间(h)', width: 70, align: 'center' },
                { field: 'TimeOutput_Day', title: '台时(t/h)', width: 70, align: 'center' },
                { field: 'RunRate_Day', title: '运转率(%)', width: 70, align: 'center' },
                { field: 'Output_Month', title: '产量（t）', width: 70, align: 'center' },
                { field: 'RunTime_Month', title: '运行时间(h)', width: 70, align: 'center' },
                { field: 'TimeOutput_Month', title: '台时(t/h)', width: 70, align: 'center' },
                { field: 'RunRate_Month', title: '运转率(%)', width: 70, align: 'center' },
                { field: 'Output_Year', title: '产量（t）', width: 70, align: 'center' },
                { field: 'RunTime_Year', title: '运行时间(h)', width: 70, align: 'center' },
                { field: 'TimeOutput_Year', title: '台时(t/h)', width: 70, align: 'center' },
                { field: 'RunRate_Year', title: '运转率(%)', width: 70, align: 'center' }
            ]]
        });
    }
}

function QueryReportFun() {
    var m_QueryDateString = $('#DatetimeF').datebox('getValue');
    var m_QueryDateNumber = m_QueryDateString.replace(/\-/g, "");
    var m_CurrentDate = new Date();
    var m_CurrentDataNumber = m_CurrentDate.getFullYear().toString() + (m_CurrentDate.getMonth() + 1).toString() + m_CurrentDate.getDate().toString();
    var m_OrganizationId = $('#OrganizationIdF').val();
    if (m_QueryDateNumber < m_CurrentDataNumber) {
        $.ajax({
            type: "POST",
            url: "DailyProduction.aspx/GetDailyProductionData",
            data: '{myOrganizationId: "' + m_OrganizationId + '", myDateTime: "' + m_QueryDateString + '"}',
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: function (msg) {
                var m_MsgData = jQuery.parseJSON(msg.d);
                if (m_MsgData != null && m_MsgData != undefined) {
                    for (var i = 0; i < m_MsgData.length; i++) {
                        var m_Output_Plan = 0.0;
                        var m_RunTime_Plan = 0.0;
                        var m_TimeOutputD_Plan = 0.0;
                        var m_RunRateD_Plan = 0.0;
                        var m_Output_Day = 0.0;
                        var m_RunTime_Day = 0.0;
                        var m_TimeOutputD_Day = 0.0;
                        var m_RunRateD_Day = 0.0;
                        var m_Output_Month = 0.0;
                        var m_RunTime_Month = 0.0;
                        var m_TimeOutputD_Month = 0.0;
                        var m_RunRateD_Month = 0.0;
                        var m_Output_Year = 0.0;
                        var m_RunTime_Year = 0.0;
                        var m_TimeOutputD_Year = 0.0;
                        var m_RunRateD_Year = 0.0;
                        for (var j = 0; j < m_MsgData[i].children.length; j++) {
                            /////////计划///////////
                            m_Output_Plan = m_Output_Plan + parseFloat(m_MsgData[i].children[j].Output_Plan);
                            m_RunTime_Plan = m_RunTime_Plan + parseFloat(m_MsgData[i].children[j].RunTime_Plan);
                            if (parseFloat(m_MsgData[i].children[j].TimeOutput_Plan) != 0) {
                                m_TimeOutputD_Plan = m_TimeOutputD_Plan + parseFloat(m_MsgData[i].children[j].Output_Plan) / parseFloat(m_MsgData[i].children[j].TimeOutput_Plan);
                            }
                            if (parseFloat(m_MsgData[i].children[j].RunRate_Plan) != 0) {
                                m_RunRateD_Plan = m_RunRateD_Plan + parseFloat(m_MsgData[i].children[j].RunTime_Plan) / parseFloat(m_MsgData[i].children[j].RunRate_Plan);
                            }
                            /////////日统计/////////
                            m_Output_Day = m_Output_Day + parseFloat(m_MsgData[i].children[j].Output_Day);
                            m_RunTime_Day = m_RunTime_Day + parseFloat(m_MsgData[i].children[j].RunTime_Day);
                            if (parseFloat(m_MsgData[i].children[j].TimeOutput_Day) != 0) {
                                m_TimeOutputD_Day = m_TimeOutputD_Day + parseFloat(m_MsgData[i].children[j].Output_Day) / parseFloat(m_MsgData[i].children[j].TimeOutput_Day);
                            }
                            if (parseFloat(m_MsgData[i].children[j].RunRate_Day) != 0) {
                                m_RunRateD_Day = m_RunRateD_Day + parseFloat(m_MsgData[i].children[j].RunTime_Day) / parseFloat(m_MsgData[i].children[j].RunRate_Day);
                            }
                            /////////月统计/////////
                            m_Output_Month = m_Output_Month + parseFloat(m_MsgData[i].children[j].Output_Month);
                            m_RunTime_Month = m_RunTime_Month + parseFloat(m_MsgData[i].children[j].RunTime_Month);
                            if (parseFloat(m_MsgData[i].children[j].TimeOutput_Month) != 0) {
                                m_TimeOutputD_Month = m_TimeOutputD_Month + parseFloat(m_MsgData[i].children[j].Output_Month) / parseFloat(m_MsgData[i].children[j].TimeOutput_Month);
                            }
                            if (parseFloat(m_MsgData[i].children[j].RunRate_Month) != 0) {
                                m_RunRateD_Month = m_RunRateD_Month + parseFloat(m_MsgData[i].children[j].RunTime_Month) / parseFloat(m_MsgData[i].children[j].RunRate_Month);
                            }
                            /////////年统计/////////
                            m_Output_Year = m_Output_Year + parseFloat(m_MsgData[i].children[j].Output_Year);
                            m_RunTime_Year = m_RunTime_Year + parseFloat(m_MsgData[i].children[j].RunTime_Year);
                            if (parseFloat(m_MsgData[i].children[j].TimeOutput_Year) != 0) {
                                m_TimeOutputD_Year = m_TimeOutputD_Year + parseFloat(m_MsgData[i].children[j].Output_Year) / parseFloat(m_MsgData[i].children[j].TimeOutput_Year);
                            }
                            if (parseFloat(m_MsgData[i].children[j].RunRate_Year) != 0) {
                                m_RunRateD_Year = m_RunRateD_Year + parseFloat(m_MsgData[i].children[j].RunTime_Year) / parseFloat(m_MsgData[i].children[j].RunRate_Year);
                            }
                        }
                        ////////////计划/////////////
                        m_MsgData[i].Output_Plan = m_Output_Plan;
                        m_MsgData[i].RunTime_Plan = m_RunTime_Plan;
                        if (m_TimeOutputD_Plan != 0) {
                            m_MsgData[i].TimeOutput_Plan = (m_Output_Plan / m_TimeOutputD_Plan).toFixed(2);
                        }
                        if (m_RunRateD_Plan != 0) {
                            m_MsgData[i].RunRate_Plan = (m_RunTime_Plan / m_RunRateD_Plan).toFixed(2);
                        }
                        ////////////日统计/////////////
                        m_MsgData[i].Output_Day = m_Output_Day;
                        m_MsgData[i].RunTime_Day = m_RunTime_Day;
                        if (m_TimeOutputD_Day != 0) {
                            m_MsgData[i].TimeOutput_Day = (m_Output_Day / m_TimeOutputD_Day).toFixed(2);
                        }
                        if (m_RunRateD_Day != 0) {
                            m_MsgData[i].RunRate_Day = (m_RunTime_Day / m_RunRateD_Day).toFixed(2);
                        }
                        ////////////月统计/////////////
                        m_MsgData[i].Output_Month = m_Output_Month;
                        m_MsgData[i].RunTime_Month = m_RunTime_Month;
                        if (m_TimeOutputD_Month != 0) {
                            m_MsgData[i].TimeOutput_Month = (m_Output_Month / m_TimeOutputD_Month).toFixed(2);
                        }
                        if (m_RunRateD_Month != 0) {
                            m_MsgData[i].RunRate_Month = (m_RunTime_Month / m_RunRateD_Month).toFixed(2);
                        }
                        ////////////年统计/////////////
                        m_MsgData[i].Output_Year = m_Output_Year;
                        m_MsgData[i].RunTime_Year = m_RunTime_Year;
                        if (m_TimeOutputD_Year != 0) {
                            m_MsgData[i].TimeOutput_Year = (m_Output_Year / m_TimeOutputD_Year).toFixed(2);
                        }
                        if (m_RunRateD_Year != 0) {
                            m_MsgData[i].RunRate_Year = (m_RunTime_Year / m_RunRateD_Year).toFixed(2);
                        }
                    }
                }
                $('#DailyProduction').treegrid("loadData", m_MsgData);
                $('#DailyProduction').treegrid("collapseAll");
            }
        });
    }
    else {
        alert("请选择今天以前的日期!");
    }
}