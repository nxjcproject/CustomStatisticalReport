/*
    上帝保佑
*/

/*
    模板class说明
    可编辑单元格：editbox
    要合计的月电量单元格：electricityQuantity_month
    要合计的电量累计单元格：electricityQuantity_accumulation
    要保存的电量单元格：electricityQuantity
    要保存的物料单元格：materialWeight
    要计算的单元格：calculateBox
    要延时计算的单元格：
*/

/*
    累计工序电耗要加calculateByEditBoxValue类  //此值为累计电量/累计产量 累计值为当月值+累计到上月的值 若没有累计到上月的值则此时 根据累计工序电耗的公式计算就会得到NaN

*/

var OrganizationId = '';
var SelectDatetime = '';
var StatisticalType = '';
var that;
var oldData;
var editHTML;
var editText;
var dataSet;//数据源，从后台获得的数据，当数据重新平衡后该数据集也相应更新
$(function () {
    InitDate();
    //LoadData();
});
//初始化日期框
function InitDate() {
    var nowDate = new Date();
    nowDate.setDate(nowDate.getDate() - 1);
    var dateString = nowDate.getFullYear() + '-' + (nowDate.getMonth()) ;
    $('#datetime').datebox('setValue', dateString);
}
//查询
function QueryReportFun() {

    LoadTemplate($('#organizationId').val());
}

function LoadTemplate(organizationId) {
    $("#contain").load("/UI_CustomStatisticalReport/ReportTemplate/balance_" + organizationId + ".html", "", myDocumentReady);
}
//html模板加载完成后执行
function myDocumentReady() {
    //mySetHtml();
    LoadData();
}
function setEditHTML(value) {
    value = value.trim();
    editHTML = '<input type="text" value="' + value + '" style="width:100%;border:0" onclick="oldData=this.value;" onblur="Onblur(this);"/>';
    //editHTML += '<input type="button" onclick="ok(this)" value="修改" />';
    //editHTML += '<input type="button" onclick="cancel(this)" value="取消" />'; 
}
//焦点失去事件
function Onblur(obj) {
    var m_newData = Number(obj.value);

    //检查是否是备注，若为备注可以不是Number类型
    var t_class = $(obj).parent().attr("class");
    var patt_remarks = new RegExp("remarks");
    var result_class = patt_remarks.exec(t_class)
    if ("remarks" != result_class) {
        if (isNaN(m_newData)) {
            $.messager.alert("警告！", "输入的数据（" + obj.value + "）有误，请重新输入！");
            obj.value = oldData;
            return;
        }
    }
    var o_parent = $(obj).parent();
    var id = o_parent.attr("id");

    var m_class = $(obj).parent().attr('class');//.class;

    var patt_elec = new RegExp("electricityQuantity_month");
    var patt_material = new RegExp("materialWeight_month");
    var patt_array = new Array();
    patt_array.push(patt_elec);
    patt_array.push(patt_material);
    var m_count = patt_array.length;
    var result_class = "";
    for (var i = 0; i < m_count; i++) {
        var result_class = patt_array[i].exec(m_class) == null ? result_class : patt_array[i].exec(m_class);
    }

    //if (id in dataSet) {
    if (result_class != "") {
        if (undefined != id && "" != id) {
            dataSet[id] = Number(obj.value);//跟新dataSet
        }
    }
    // }

    CalculateTotal(result_class);
    if (CalculateBoxValue("calculateBox")) {
        CalculateBoxValue("delayCalculateBox");
    }
    CalculateByEditBoxValue("calculateByEditBoxValue");
    CalculateTotal("electricityQuantity_accumulation");//当当月值修改时累计值合计也要相应修改
}
//计算合计
function CalculateTotal(myClass) {
    var m_class = myClass;
    if ("" == myClass) {
        return;
    }
    var m_total = 0;
    $("." + m_class).each(function () {
        //m_total += Number($(this).children().attr("value"));
        m_total += Number($(this).html());
    });
    $("." + m_class + "_sum").each(function () {
        //$(this).children().attr("value", m_total.toFixed(2));
        $(this).attr("value", m_total.toFixed(2));
    });
    //$(".electricityQuantity_month_sum").attr("value", m_total);
}
//绑定事件
function mySetHtml() {
    $(".editbox").each(function () { //取得所有class为editbox的对像
        editText = $(this).html(); //取得表格单元格的文本
        setEditHTML(editText);
        $(this).html(editHTML);
    });

};
//加载数据
function LoadData() {
    var startDate = "";
    var endDate = "";

    //var o_nowGrid = $('#nowId').combogrid('grid');	// get datagrid object
    //var nowSelectRow = o_nowGrid.datagrid('getSelected');	// get the selected row

    //var o_beforeGrid = $('#beforeId').combogrid('grid');	// get datagrid object
    //var beforeSelectRow = o_beforeGrid.datagrid('getSelected');	// get the selected row

    StatisticalType = $("#cc").combobox("getValue");

    var queryDate = $("#datetime").datebox("getValue");//查询时间
    var win = $.messager.progress({
        title: '请稍后',
        msg: '数据载入中...'
    });
    $.ajax({
        type: "POST",
        url: "report_QueryBalanceStatistics.aspx/GetReportDataSet",
        data: '{organizationId: "' + OrganizationId + '", dateTime: "' + queryDate + '",statisticalType:"' + StatisticalType + '"}',
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (msg) {
            $.messager.progress('close');
            dataSet = msg.d;
            FillCell(msg.d);
        },
        beforeSend: function (XMLHttpRequest) {
            win;
        }
    });
}
//填充单元格
function FillCell(myData) {
    for (var item in myData) {
        var m_obj = $(document.getElementById(item));//ID中有特殊字符，jquery不支持，先有js获得对象再转换成jquery对象      
        if (m_obj.length != 0) {
            var value = Number(myData[item]).toFixed(2) == "NaN" ? myData[item] : Number(myData[item]).toFixed(2);
            //m_obj.children().attr("value", value);//保留两位小数
            m_obj.html(value);//保留两位小数
        }
    }
    CalculateTotal("electricityQuantity_month");//计算月电量合计

    if (CalculateBoxValue("calculateBox")) {
        CalculateBoxValue("delayCalculateBox");//延时计算
        CalculateByEditBoxValue("calculateByEditBoxValue");//
    }

    //当累计值计算完之后再计算累计合计值，故放到最后
    CalculateTotal("electricityQuantity_accumulation");//计算累计电量合计
}

//计算电耗
function CalculateBoxValue(className) {
    if ("" == className) {
        return;
    }
    $("." + className).each(function () {
        var m_formula = $(this).attr("data-formula");
        //$(this).children().attr("value", m_formula);
        if (m_formula != undefined) {
            var formulaArray = new FormulaParser(m_formula);
            //用数值替换因子
            var i_count = formulaArray.length;
            for (var i = 0; i < i_count; i++) {
                if (formulaArray[i] in dataSet) {//如果数据源集合中有就从数据源取数
                    m_formula = m_formula.replace(formulaArray[i], dataSet[formulaArray[i]]);
                }
                else {
                    m_formula = m_formula.replace(formulaArray[i], 0);
                }
                //else {//否则从单元格中取数
                //    var t_value;
                //    var originalObj = document.getElementById(formulaArray[i]);
                //    if (originalObj == null) {
                //        t_value = 0;
                //    }
                //    else {
                //        var m_obj = $(originalObj);
                //        t_value = $(originalObj).children().attr("value");
                //        t_value = (t_value.trim() == "" || t_value == null || t_value == undefined) ? 0 : t_value;
                //    }
                //    m_formula = m_formula.replace(formulaArray[i], t_value);
                //}
            }
            var t_value = m_formula == undefined ? 0 : (eval(m_formula = m_formula == "" ? 0 : m_formula).toFixed(2));
            t_value = t_value == Infinity ? 0 : t_value;
            //$(this).children().attr("value", t_value);//计算公式值并赋值给单元格
            $(this).html(t_value);//计算公式值并赋值给单元格
        }
    });
    return true;
}

//只从单元格中取值
function CalculateByEditBoxValue(className) {
    if ("" == className) {
        return;
    }
    $("." + className).each(function () {
        var m_formula = $(this).attr("data-formula");
        //$(this).children().attr("value", m_formula);
        if (m_formula != undefined) {
            var formulaArray = new FormulaParser(m_formula);
            //用数值替换因子
            var i_count = formulaArray.length;
            for (var i = 0; i < i_count; i++) {
                var t_value;
                var originalObj = document.getElementById(formulaArray[i]);
                if (originalObj == null) {
                    t_value = 0;
                }
                else {
                    var m_obj = $(originalObj);
                    //t_value = $(originalObj).children().attr("value");
                    t_value = $(originalObj).html();
                    t_value = (t_value.trim() == "" || t_value == null || t_value == undefined) ? 0 : t_value;
                }
                m_formula = m_formula.replace(formulaArray[i], t_value);
            }
        }
        var t_value = m_formula == undefined ? 0 : (eval(m_formula = m_formula == "" ? 0 : m_formula).toFixed(2));
        t_value = t_value == Infinity ? 0 : t_value;
        //$(this).children().attr("value", t_value);//计算公式值并赋值给单元格
        $(this).html(t_value);//计算公式值并赋值给单元格
    }
    );
    return true;
}

//延时计算
//function DelayCalculateBoxValue(className) {
//    $("." + className).each(function () {
//        var m_formula = $(this).attr("data-formula");
//        //$(this).children().attr("value", m_formula);
//        if (m_formula != undefined) {
//            var formulaArray = new FormulaParser(m_formula);
//            //用数值替换因子
//            var i_count = formulaArray.length;
//            for (var i = 0; i < i_count; i++) {
//                var t_value;
//                var originalObj = document.getElementById(formulaArray[i]);
//                if (originalObj == null) {
//                    t_value = 0;
//                }
//                else {
//                    var m_obj = $(originalObj);
//                    t_value = $(originalObj).children().attr("value");
//                    t_value = (t_value.trim() == "" || t_value == null || t_value == undefined) ? 0 : t_value;
//                }
//                m_formula = m_formula.replace(formulaArray[i], t_value);
//            }
//            var resultValue=eval(m_formula = m_formula == "" ? 0: m_formula).toFixed(2);
//            resultValue=resultValue=="NaN"?Number(0).toFixed(2):resultValue;
//            $(this).children().attr("value", resultValue);//计算公式值并赋值给单元格
//        }
//    });
//}

//保存
//function Save() {
//    //当电量产量保存成功时才能保存备注信息
//    //当保存失败或数据已经存在的时候不能保存备注信息
//    if (1 == SaveData()) {
//        SaveRemarks();
//    }
//}


//保存成功返回1，其他返回2
function SaveData() {
    var t_dataset = "";
    var sendData;
    var upperClass;
    //获取要保存的电量
    $(".electricityQuantity,.materialWeight,.remarks").each(function () {
        var cur_id = $(this).attr("id");
        var t_class = $(this).attr("class");
        //
        var patt_elec = new RegExp("electricityQuantity");
        var patt_material = new RegExp("materialWeight");
        var patt_remarks = new RegExp("remarks");
        var patt_array = new Array();
        patt_array.push(patt_elec);
        patt_array.push(patt_material);
        patt_array.push(patt_remarks);

        var m_count = patt_array.length;
        var result_class = "";
        for (var i = 0; i < m_count; i++) {
            var result_class = patt_array[i].exec(t_class) == null ? result_class : patt_array[i].exec(t_class);
        }
        //
        result_class = result_class.toString();
        //首字母转换为大写
        upperClass = result_class.substring(0, 1).toUpperCase() + result_class.substring(1, result_class.length);
        if (cur_id != undefined) {
            t_dataset += cur_id + ">" + upperClass + ":"
            //var cur_value = $(this).children().attr("value");
            var cur_value = $(this).attr("value");
            cur_value = (cur_value.trim() == "" || cur_value == null || cur_value == undefined) ? 0 : cur_value.replace(/"([^"]*)"/g, " ");
            t_dataset += cur_value + ",";
        }
    })
    t_dataset = t_dataset.substring(0, t_dataset.length - 1);//去掉字符串最后一个字符
    var saveDate = $("#saveDateTime").datebox("getValue");
    if ('' == saveDate) {
        $.messager.alert("提示", "请选择保存时间！");
        return;
    }
    sendData = "{dateTime:'" + saveDate + "',organizationId:'" + OrganizationId + "',statisticalType:'" + StatisticalType + "',dataSet:'" + t_dataset + "'}";
    $.ajax({
        type: "POST",
        url: "report_BalanceStatistics.aspx/SaveData",
        data: sendData,
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (msg) {
            $.messager.alert("提示", msg.d);
            if ("保存成功" == msg.d) {
                return 1;
            }
            else {
                return 2;
            }
        }
    });
}

//目录树双击事件
function onOrganisationTreeClick(node) {
    $('#productLineName').textbox('setText', node.text);
    $('#organizationId').val(node.OrganizationId);
    OrganizationId = node.OrganizationId;
    LoadCombogrid(node.OrganizationId);
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

function LoadCombogrid(organizationId) {

    $.ajax({
        type: "POST",
        url: "report_BalanceStatistics.aspx/GetMeterReadingValue",
        data: "{organizationId:'" + organizationId + "'}",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (msg) {
            var myData = JSON.parse(msg.d);
            $('#beforeId').combogrid({
                panelWidth: 200,
                panelHeight: true,
                //value: '006',
                idField: 'DataItemId',
                textField: 'DataValue',
                data: myData["rows"],
                columns: [[
                { field: 'TimeStamp', title: '时间', width: 90 },
                { field: 'DataValue', title: '数值', width: 100 }
                ]]
            });

            $('#nowId').combogrid({
                panelWidth: 200,
                panelHeight: true,
                idField: 'DataItemId',
                textField: 'DataValue',
                data: myData["rows"],
                columns: [[
                { field: 'TimeStamp', title: '时间', width: 90 },
                { field: 'DataValue', title: '数值', width: 100 }
                ]]
            });
        }
    });
}

function CalculateDiffValue() {
    var beforeValue = 0;
    var lastValue = 0;
    var g = $('#beforeId').combogrid('grid');	// get datagrid object
    var h = $('#nowId').combogrid('grid');	// get datagrid object
    var r = g.datagrid('getSelected');	// get the selected row
    var s = h.datagrid('getSelected');	// get the selected row
    beforeValue = (r == null ? 0 : Number(r.DataValue));
    lastValue = (s == null ? 0 : Number(s.DataValue));
    $("#diffValueId").text(lastValue - beforeValue);
}



function ExportFileFun() { 

    var m_FunctionName = "ExcelStream";
    var m_Parameter1 = $("#contain").html();
    var m_Parameter2 = "";

    var m_ReplaceAlllt = new RegExp("<", "g");
    var m_ReplaceAllgt = new RegExp(">", "g");
    m_Parameter1 = m_Parameter1.replace(m_ReplaceAlllt, "&lt;");
    m_Parameter1 = m_Parameter1.replace(m_ReplaceAllgt, "&gt;");

    var form = $("<form id = 'ExportFile'>");   //定义一个form表单
    form.attr('style', 'display:none');   //在form表单中添加查询参数
    form.attr('target', '');
    form.attr('method', 'post');
    form.attr('action', "report_QueryBalanceStatistics.aspx");

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
    var m_ReportTableHtml = $("#contain").html();
    PrintHtml(m_ReportTableHtml);
}

//从单元格中移除input
function removeInputHtml() {
    $(".editbox").each(function () { //取得所有class为editbox的对像
        var inputValue = $(this).children().attr("value");
        $(this).html(inputValue);
    });

};