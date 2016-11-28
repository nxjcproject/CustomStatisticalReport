
$(function () {
    InitDayTime();
    LoadHtml(g_templateURL);

});
//初始化时间
function InitDayTime() {
    var nowDate = new Date();   
    var dateString = nowDate.getFullYear() + '-' + (nowDate.getMonth() + 1) + '-' + (nowDate.getDate() - 1);
    $('#datetime').datebox('setValue', dateString);


    //var nowDate = new Date();
    //var beforeDate = new Date();
    //beforeDate.setDate(nowDate.getDate() - 10);
    //var nowString = nowDate.getFullYear() + '-' + (nowDate.getMonth() + 1) + '-' + nowDate.getDate() + " " + nowDate.getHours() + ":" + nowDate.getMinutes() + ":" + nowDate.getSeconds();
    //var beforeString = beforeDate.getFullYear() + '-' + (beforeDate.getMonth() + 1) + '-' + beforeDate.getDate() + " 00:00:00";
    //$('#startDate').datetimebox('setValue', beforeString);
    //$('#endDate').datetimebox('setValue', nowString);
}
//加载报表模板
function LoadHtml(t_url)
{
    $("#contain").load(t_url);
}
