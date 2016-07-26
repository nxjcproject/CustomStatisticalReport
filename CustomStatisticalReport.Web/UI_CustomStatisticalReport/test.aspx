<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="test.aspx.cs" Inherits="CustomStatisticalReport.Web.UI_CustomStatisticalReport.test" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title></title>
    <link rel="stylesheet" type="text/css" href="/lib/ealib/themes/gray/easyui.css" />
    <link rel="stylesheet" type="text/css" href="/lib/ealib/themes/icon.css" />
    <link rel="stylesheet" type="text/css" href="/lib/extlib/themes/syExtIcon.css" />
    <link rel="stylesheet" type="text/css" href="/lib/extlib/themes/syExtCss.css" />

    <script type="text/javascript" src="/lib/ealib/jquery.min.js" charset="utf-8"></script>
    <script type="text/javascript" src="/lib/ealib/jquery.easyui.min.js" charset="utf-8"></script>
    <script type="text/javascript" src="/lib/ealib/easyui-lang-zh_CN.js" charset="utf-8"></script>

    <!--[if lt IE 8 ]><script type="text/javascript" src="/js/common/json2.min.js"></script><![endif]-->
    <script type="text/javascript" src="/js/common/PrintFile.js" charset="utf-8"></script>
</head>
<body>
    <form id="form1" runat="server">
        <div>
            <%--<table border="1">
                <tr>
                    <th>姓名</th>
                    <th colspan="2" class="editbox">电话</th>
                </tr>
                <tr>
                    <td>Bill Gates</td>
                    <td>555 77 854</td>
                    <td>555 77 855</td>
                </tr>
                <tr>
                    <td rowspan="2" class="editbox">david</td>
                    <td rowspan="2">123456</td>
                    <td>werwefd</td>
                </tr>
                <tr>
                    <td>6789</td>
                </tr>
            </table>

            <table border="1" width="80%">
                <tr id="tr_11">
                    <td class="editbox">姓名1</td>
                    <td>性别1</td>
                    <td class="editbox">年龄1</td>
                    <td>生活1</td>
                </tr>
                <tr id="tr_22">
                    <td class="editbox">姓名2</td>
                    <td>性别2</td>
                    <td class="editbox">年龄2</td>
                    <td>生活2</td>
                </tr>
                <tr id="tr_33">
                    <td class="editbox">姓名3</td>
                    <td>性别3</td>
                    <td class="editbox">年龄3</td>
                    <td>生活3</td>
                </tr>
                <tr id="tr_44">
                    <td class="editbox">姓名4</td>
                    <td>性别4</td>
                    <td class="editbox">年龄4</td>
                    <td>生活4</td>
                </tr>
            </table>--%>
            <div id="contain" style="width: 100px"></div>
            <script type="text/javascript" charset="utf-8">
               
            </script>
            <style type="text/css">
                td {
                    text-align:center;
                    padding:0;
                    margin:0;
                }
                /*table {
                    margin: 0 auto;
                    text-align:center
                }*/
            </style>
            <script type="text/javascript">
                var that;
                $("#contain").load("111.html", "", mySetHtml);
                var oldData;
                var editHTML;
                var editText;
                function setEditHTML(value) {
                    value = value.trim();
                    editHTML = '<input type="text" value="' + value + '" style="width:100%;border:0" onclick="oldData=this.value;" onblur="Onblur(this);"/>';
                    //editHTML += '<input type="button" onclick="ok(this)" value="修改" />';
                    //editHTML += '<input type="button" onclick="cancel(this)" value="取消" />'; 
                }
                //焦点失去事件
                function Onblur(obj) {
                    var m_newData = Number(obj.value);
                    if (isNaN(m_newData)) {
                        $.messager.alert("警告！","输入的数据（" + obj.value + "）有误，请重新输入！");
                        obj.value = oldData;
                        return;
                    }
                    var m_class = $(obj).parent().attr('class');//.class;
                    m_class = m_class.replace("editbox", "");//从多个类中去掉"editbox"类
                    m_class = m_class.replace(" ", "");//去掉空格
                    CalculateTotal(m_class);

                }
                //计算合计
                function CalculateTotal(myClass) {
                    var m_class = myClass;
                    var m_total = 0;
                    $("."+m_class).each(function () {
                        m_total += Number($(this).children().attr("value"));
                    });
                    $("."+m_class+"_sum").each(function () {
                        $(this).children().attr("value", m_total);
                    });
                    //$(".electricityQuantity_month_sum").attr("value", m_total);
                }
                //绑定事件
               function mySetHtml() {
                   $(".editbox").each(function () { //取得所有class为editbox的对像
                        editText = $(this).html(); //取得表格单元格的文本
                        setEditHTML(editText);
                        $(this).html(editHTML);
                       // $(this).html('<input type="text" value="input" style="width:100%;border:0"/>');
                        //$(this).bind("dblclick", function () { //给其绑定双击事件
                        //    //if (that != this) {
                        //    //    alert("ok");
                        //    //}
                        //    editText = $(this).html(); //取得表格单元格的文本
                        //    setEditHTML(editText); //初始化控件
                        //    $(this).data("oldtxt", editText) //将单元格原文本保存在其缓存中，便修改失败或取消时用
                        //    .html(editHTML) //改变单元格内容为编辑状态
                        //    .unbind("dblclick"); //删除单元格双击事件，避免多次双击
                        //});
                    });
                };

                //取消
                function cancel(cbtn) {
                    var $obj = $(cbtn).parent(); //'取消'按钮的上一级，即单元格td
                    $obj.html($obj.data("oldtxt")); //将单元格内容设为原始数据，取消修改
                    $obj.bind("dblclick", function () { //重新绑定单元格双击事件
                        editText = $(this).html();
                        setEditHTML(editText);
                        $(this).data("oldtxt", editText)
                        .html(editHTML).unbind("dblclick");
                    });
                }

                //修改
                function ok(obtn) {
                    var $obj = $(obtn).parent(); //'修改'按钮的上一级，即单元格td
                    var id = $obj.parent().attr("id").replace("tr_", ""); //取得该行数据的ID，此例ID绑定在tr中
                    var value = $obj.find("input:text")[0].value; //取得文本框的值，即新数据

                    //AJAX 修改数据略
                    //成功
                    if (true) {
                        alert("success");
                        $obj.data("oldtxt", value); //设置此单元格缓存为新数据
                        cancel(obtn); //调用'取消'方法，
                        //在此应传'取消'按钮过去，
                        //但在'取消'事件中没有用'取消'按钮这个对 象,
                        //用的只是它的上一级，即td，
                        //固在此直接用'修改'按钮替代
                    }
                        //失败
                    else {
                        alert("error");
                        cancel(obtn);
                    }

                }

            </script>

        </div>
    </form>
</body>
</html>
