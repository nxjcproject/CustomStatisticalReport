$(function () {
    Loadtreegrid("first", { "rows": [], "total": 0 });
    InitDate();

});
//初始化日期框
function InitDate() {
    var nowDate = new Date();
    nowDate.setDate(nowDate.getDate() - 1);
    var dateString = nowDate.getFullYear() + '-' + (nowDate.getMonth());
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
    var m_QueryDate = $('#DatetimeF').datebox('getValue');
    var m_OrganizationId = $('#OrganizationIdF').val();
    $.ajax({
        type: "POST",
        url: "DailyProduction.aspx/GetDailyProductionData",
        data: '{myOrganizationId: "' + m_OrganizationId + '", myDateTime: "' + m_QueryDate + '"}',
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (msg) {
            var m_MsgData = jQuery.parseJSON(msg.d);
            if (m_MsgData != null && m_MsgData != undefined) {
                for (var i = 0; i < m_MsgData.Length; i++) {
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
                    for (var j = 0; j < m_MsgData[i].chieldren.Length; j++) {
                        /////////计划///////////
                        m_Output_Plan = m_Output_Plan + m_MsgData[i].chieldren[j].Output_Plan;
                        m_RunTime_Plan = m_RunTime_Plan + m_MsgData[i].chieldren[j].RunTime_Plan;
                        if (m_TimeOutputD_Plan != 0) {
                            m_TimeOutputD_Plan = m_TimeOutputD_Plan + m_MsgData[i].chieldren[j].Output_Plan / m_MsgData[i].chieldren[j].m_TimeOutput_Plan;
                        }
                        if (m_RunRateD_Plan != 0) {
                            m_RunRateD_Plan = m_RunRateD_Plan + m_MsgData[i].chieldren[j].m_RunTime_Plan / m_MsgData[i].chieldren[j].m_RunRate_Plan;
                        }
                        /////////日统计/////////
                        m_Output_Day = m_Output_Day + m_MsgData[i].chieldren[j].Output_Day;
                        m_RunTime_Day = m_RunTime_Day + m_MsgData[i].chieldren[j].RunTime_Day;
                        if (m_TimeOutputD_Day != 0) {
                            m_TimeOutputD_Day = m_TimeOutputD_Day + m_MsgData[i].chieldren[j].Output_Day / m_MsgData[i].chieldren[j].m_TimeOutput_Day;
                        }
                        if (m_RunRateD_Day != 0) {
                            m_RunRateD_Day = m_RunRateD_Day + m_MsgData[i].chieldren[j].m_RunTime_Day / m_MsgData[i].chieldren[j].m_RunRate_Day;
                        }
                        /////////月统计/////////
                        m_Output_Month = m_Output_Month + m_MsgData[i].chieldren[j].Output_Month;
                        m_RunTime_Month = m_RunTime_Month + m_MsgData[i].chieldren[j].RunTime_Month;
                        if (m_TimeOutputD_Month != 0) {
                            m_TimeOutputD_Month = m_TimeOutputD_Month + m_MsgData[i].chieldren[j].Output_Month / m_MsgData[i].chieldren[j].m_TimeOutput_Month;
                        }
                        if (m_RunRateD_Month != 0) {
                            m_RunRateD_Month = m_RunRateD_Month + m_MsgData[i].chieldren[j].m_RunTime_Month / m_MsgData[i].chieldren[j].m_RunRate_Month;
                        }
                        /////////年统计/////////
                        m_Output_Year = m_Output_Year + m_MsgData[i].chieldren[j].Output_Year;
                        m_RunTime_Year = m_RunTime_Year + m_MsgData[i].chieldren[j].RunTime_Year;
                        if (m_TimeOutputD_Year != 0) {
                            m_TimeOutputD_Year = m_TimeOutputD_Year + m_MsgData[i].chieldren[j].Output_Year / m_MsgData[i].chieldren[j].m_TimeOutput_Year;
                        }
                        if (m_RunRateD_Year != 0) {
                            m_RunRateD_Year = m_RunRateD_Year + m_MsgData[i].chieldren[j].m_RunTime_Year / m_MsgData[i].chieldren[j].m_RunRate_Year;
                        }
                    }
                    ////////////计划/////////////
                    m_MsgData[i].Output_Plan = m_Output_Plan;
                    m_MsgData[i].RunTime_Plan = m_RunTime_Plan;
                    if(m_TimeOutputD_Plan != 0)
                    {
                        m_MsgData[i].m_TimeOutput_Plan = m_Output_Plan / m_TimeOutputD_Plan;
                    }
                    if (m_RunRateD_Plan != 0) {
                        m_MsgData[i].m_RunRate_Plan = m_Output_Plan / m_RunRateD_Plan;
                    }
                    ////////////日统计/////////////
                    m_MsgData[i].Output_Day = m_Output_Day;
                    m_MsgData[i].RunTime_Day = m_RunTime_Day;
                    if (m_TimeOutputD_Day != 0) {
                        m_MsgData[i].m_TimeOutput_Day = m_Output_Day / m_TimeOutputD_Day;
                    }
                    if (m_RunRateD_Day != 0) {
                        m_MsgData[i].m_RunRate_Day = m_Output_Day / m_RunRateD_Day;
                    }
                    ////////////日统计/////////////
                    m_MsgData[i].Output_Month = m_Output_Month;
                    m_MsgData[i].RunTime_Month = m_RunTime_Month;
                    if (m_TimeOutputD_Month != 0) {
                        m_MsgData[i].m_TimeOutput_Month = m_Output_Month / m_TimeOutputD_Month;
                    }
                    if (m_RunRateD_Month != 0) {
                        m_MsgData[i].m_RunRate_Month = m_Output_Month / m_RunRateD_Month;
                    }
                    ////////////日统计/////////////
                    m_MsgData[i].Output_Year = m_Output_Year;
                    m_MsgData[i].RunTime_Year = m_RunTime_Year;
                    if (m_TimeOutputD_Year != 0) {
                        m_MsgData[i].m_TimeOutput_Year = m_Output_Year / m_TimeOutputD_Year;
                    }
                    if (m_RunRateD_Year != 0) {
                        m_MsgData[i].m_RunRate_Year = m_Output_Year / m_RunRateD_Year;
                    }
                }
            }
            $('#DailyProduction').treegrid("loadData", m_MsgData);
            $('#DailyProduction').treegrid("collapseAll");
        }
    });
}
