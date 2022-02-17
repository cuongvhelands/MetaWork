var boards = [], items = [], KanbanTest, itemIdMax = 0, LstBoards = [], issueid;


$.ajax({
    url: '/api/Sprint/GetIssuesInProjectSprint/' + projectSprintId + '/' + projectId + "/" + assgine,
    type: 'GET',
    contentType: "application/json; charset=utf-8",
    async: false,
    success: function (data) {
        //SprintAjax.GetIssuesInProjectSprint(projectInfo, function (data) {
        if (data && data.IssueStatuss.length > 0) {
            $.each(data.IssueStatuss, function (index, value) {
                var item2Board = [];
                for (i = 0; i < data.Issues.length; i++) {
                    var key = "";
                    var item = data.Issues[i];
                    if (item.Parent) {
                        key = item.Parent.IssueKey;
                    }
                    var userAvatar = "";
                    if (item.Assign) userAvatar = item.Assign.HoTenNguoiDung;
                    if (item.IssueStatusId == value.IssueStatusId) {
                        var itemone = {
                            "id": "item" + item.IssueId,
                            "title": getHtmlCard(item.IssueId, key, item.IssueKey, item.Summary, item.IssueType.Avatar, item.Estimate, item.IssuePriority.Avatar, userAvatar),
                            "drop": function (el, target, source, sibling) {
                                dropItem(el.dataset.eid, source.parentElement.dataset.id, target.parentElement.dataset.id);
                            }
                        }
                        item2Board.push(itemone);
                        items.push(itemone);
                    }
                }
                var dragTo = [];
                if (value.StatusWorkflow && value.StatusWorkflow != "" && value.StatusWorkflow != "[]") {
                    var lstdragTo = JSON.parse(value.StatusWorkflow);
                    for (k = 0; k < lstdragTo.length; k++) {
                        dragTo.push("board" + lstdragTo[k])
                    }
                    boards.push({
                        "id": "board" + value.IssueStatusId,
                        "title": '<label class="boardTitle">' + value.Name + '<span id="spanCountRecord2Board' + value.IssueStatusId + '"> ' + item2Board.length + '</span></label><button type="button" class="btn btn-link collapseBoard" id="btnCollapsId' + value.IssueStatusId + '"><i class="fas fa-angle-double-left"></i></button><button type="button" class="btn btn-link expandBoard" id="btnExpandsId' + value.IssueStatusId + '"><i class="fas fa-angle-double-right"></i></button>',
                        //"class": value.Class,
                        "class": "board-item-title",
                        "dragTo": dragTo,
                        "item": item2Board,

                    })
                } else {
                    boards.push({
                        "id": "board" + value.IssueStatusId,
                        "title": '<label class="boardTitle">' + value.Name + '<span id="spanCountRecord2Board' + value.IssueStatusId + '"> ' + item2Board.length + '</span></label><button type="button" class="btn btn-link collapseBoard" id="btnCollapsId' + value.IssueStatusId + '"><i class="fas fa-angle-double-left"></i></button><button type="button" class="btn btn-link expandBoard" id="btnExpandsId' + value.IssueStatusId + '"><i class="fas fa-angle-double-right"></i></button>',
                        //"class": value.Class,
                        "class": "board-item-title",
                        "item": item2Board,

                    })
                }


                LstBoards.push({ BoardId: "board" + value.IssueStatusId, DragTo: dragTo, Title: value.Title, Items: item2Board, Class: value.Class, WorkflowActions: value.WorkflowActions })
            });
            KanbanTest = new jKanban({
                element: '#myKanban',
                gutter: '',
                widthBoard: '',
                dragBoards: false,
                boards: boards,
                click: function (el) {
                    var itemId = el.dataset.eid.replace("item", "");
                    clickItem(itemId);
                },
            });
            var lst = $(".avatarNguoiDungCard");
            if (lst.length > 0) {
                $.each(lst, function (index, value) {
                    var id = this.id;
                    var name = $.trim(this.title);
                    GLOBAL.utils.SetAvatarNguoiDung(name, id);
                })
            }

        }
    },
    error: function (xhr, ajaxOptions, thrownError) {
        alert(xhr);
        alert(ajaxOptions);
        alert(thrownError);

    }
})