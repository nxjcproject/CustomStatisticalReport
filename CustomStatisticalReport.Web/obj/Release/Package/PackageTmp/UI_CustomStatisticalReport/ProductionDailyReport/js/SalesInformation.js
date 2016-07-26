$(function () {
    //Loaddatagrid("first");
    InitDate();

});
//初始化日期框
function InitDate() {
    var nowDate = new Date();
    nowDate.setDate(nowDate.getDate() - 1);
    var dateString = nowDate.getFullYear() + '-' + (nowDate.getMonth());
    $('#datetime').datebox('setValue', dateString);
}

//function Loaddatagrid(type, myData) {
//    if (type == "first") {
//        $('#SalesInformation').datagrid({
//            toolbar: '#toolbar_head',
//            singleSelect: true,
//            rownumbers:true,
//            columns: [
//                [
//                 { title: '公司', rowspan: 2, width: 180, align: 'center' },
//                 { title: '分厂', rowspan: 2, width: 180, align: 'center' },
//                 { title: '产品', rowspan: 2, width: 180, align: 'center' },
//                 //{ title: '销售计划', colspan: 2, width: 300, align: 'center' },
//                 { title: '期末库存', colspan: 2, width: 300, align: 'center' }
//                 //{ field: 'MaxInventory', title: '最大库存', rowspan: 2, width: 150, align: 'center' }
//            ], [
//                 { field: 'Beginventory', title: '月初库存', width: 150, align: 'center' },
//                 //{ field: 'YearBeinventory', title: '年初库存', width: 150, align: 'center' },
//                 //{ field: 'Endinventory', title: '月末库存', width: 150, align: 'center' },
//                 { field: 'YearEndInventory', title: '年末库存', width: 150, align: 'center' }
//            ]
//            //, [{ width: 180, align: 'center' },
//            //     {  width: 180, align: 'center' }], [{  width: 180, align: 'center' },
//            //     {  width: 180, align: 'center' }], [{  width: 180, align: 'center' },
//            //     { width: 180, align: 'center' }], [{  width: 180, align: 'center' },
//            //     {  width: 180, align: 'center' }]
//            ]
//            //,
//            //data: [{
//            //    id: 1,
//            //    factory: "二分厂",
//            //    children: [{ id: 2, factory: "熟料库" }, { id: 3, factory: "生料库" }]
//            //}]
//        });
//    }
//    else if (type == "last") {

//    }
//}
