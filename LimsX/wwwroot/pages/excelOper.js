layui.config({
    base: '../layuiModule/'
}).extend({
    treeSelect: 'treeSelect/treeSelect'
});
layui.use(['form', 'layer', 'laydate', 'table', 'laytpl', 'treeSelect'], function () {
    var form = layui.form,
        layer = parent.layer === undefined ? layui.layer : top.layer,
        $ = layui.jquery,
        laydate = layui.laydate,
        laytpl = layui.laytpl,
        table = layui.table;
    var treeSelect = layui.treeSelect;

    treeSelect.render({
        // 选择器
        elem: '#orgTree',
        // 数据
        data: 'excelOper?handler=OrgTree',
        type: "post",
        // 占位符
        placeholder: '修改默认提示信息',
        // 是否开启搜索功能：true/false，默认false
        search: true,
        // 一些可定制的样式
        style: {
            folder: {
                enable: true
            },
            line: {
                enable: true
            }
        },
        // 点击回调
        click: function (d) {
            console.log(d);
        },
        // 加载完成后的回调函数
        success: function (d) {
            console.log(d);
            //                选中节点，根据id筛选
            treeSelect.checkNode('tree', 3);
            console.log($('#tree').val());
            //                获取zTree对象，可以调用zTree方法
            var treeObj = treeSelect.zTree('tree');
            console.log(treeObj);
            //                刷新树结构
            treeSelect.refresh('tree');
        }
    });

    form.on('select(hc_select)', function (data) {   //选择移交单位 赋值给input框
        $("#HandoverCompany").val(data.value);
        $("#hc_select").next().find("dl").css({ "display": "none" });
        form.render();
    });

    window.search = function () {
        var value = $("#tplSelect").val();
        $("#tplName").val(value);
        form.render();
        $("#tplSelect").next().find("dl").css({ "display": "block" });
        var dl = $("#tplSelect").next().find("dl").children();
        var j = -1;
        for (var i = 0; i < dl.length; i++) {
            if (dl[i].innerHTML.indexOf(value) <= -1) {
                dl[i].style.display = "none";
                j++;
            }
            if (j == dl.length - 1) {
                $("#tplSelect").next().find("dl").css({ "display": "none" });
            }
        }

    }

    $("#test").click(function () {
        $.ajax({
            url: "./excelOper",
            type: "post",
            async: false,
            success: function (ret) {
                alert(ret);
            }
        });
    });

    ////新闻列表
    //var tableIns = table.render({
    //    elem: '#mainTable',
    //    url: './excelOper',
    //    cellMinWidth: 95,
    //    page: true,
    //    height: "full-125",
    //    limit: 20,
    //    limits: [10, 15, 20, 25],
    //    id: "newsListTable",
    //    cols: [[
    //        { type: "checkbox", fixed: "left", width: 50 },
    //        { field: 'newsId', title: 'ID', width: 60, align: "center" },
    //        { field: 'newsName', title: '文章标题', width: 350 },
    //        { field: 'newsAuthor', title: '发布者', align: 'center' },
    //        { field: 'newsStatus', title: '发布状态', align: 'center', templet: "#newsStatus" },
    //        { field: 'newsLook', title: '浏览权限', align: 'center' },
    //        {
    //            field: 'newsTop', title: '是否置顶', align: 'center', templet: function (d) {
    //                return '<input type="checkbox" name="newsTop" lay-filter="newsTop" lay-skin="switch" lay-text="是|否" ' + d.newsTop + '>'
    //            }
    //        },
    //        {
    //            field: 'newsTime', title: '发布时间', align: 'center', minWidth: 110, templet: function (d) {
    //                return d.newsTime.substring(0, 10);
    //            }
    //        },
    //        { title: '操作', width: 170, templet: '#newsListBar', fixed: "right", align: "center" }
    //    ]]
    //});


})