
var $header = $('.frame-search');
const $footer = $('.frame-footer');
const $table = $('#TBLDANHSACH');
const $tableBody = $table.find('tbody');
const $modal = $('#CHITIET');
const $router = "Admin/Product";
const $form = $('#ModalForm');


function DataSearch(pageNum) {
    this.Ten = $header.find('[name=Ten]').val().trim();
    this.Ma = $header.find('[name=Ma]').val().trim();
    this.PageNum = pageNum || $footer.find('[name=PageNumber]').val();;
    this.PageSize = $footer.find('[name=PageLength]').val();
}

$(function () {
    var $page = {
        init: function () {
            $page.initValidate();
            $page.initGiaoDien();
            $page.BtnSearchClick();
            $page.GetList(1);
        },
        initValidate: function () {
            $form.validate({
                rules: {
                    Name: {
                        required: true
                    }
                },
                messages: {
                    Name: {
                        required: "Tên danh mục không được để trống"
                    }
                }
            })
        },
        initGiaoDien: function () {
            $('#descriptionEditor').summernote({
                height: 300,
                placeholder: 'Nhập mô tả sản phẩm...',
                toolbar: [
                    ['style', ['bold', 'italic', 'underline']],
                    ['para', ['ul', 'ol', 'paragraph']],
                    ['insert', [/*'picture', */'link']],
                    /* ['view', ['fullscreen', 'codeview']]*/
                ]
            });

            $modal.find("#fileUpload").fileUpload();
        },
        Self: $('.card'),
        BtnSearchClick: function () {
            $header.find('.btnSearch').on('click', function () { $page.GetList(1); });
        },
        GetList: function (pageNum = 1) {

            let html = '';
            let search = new DataSearch(pageNum);

            var getResponse = AjaxConfigHelper.SendRequestToServer(`/${$router}/GetTable`, "GET", search);
            getResponse.then((res) => {
                if (res.IsOk) {
                    let data = res.Body.Data || [];

                    if (data.length == 0) {
                        html = `<tr><td class="text-center" colspan="5"><span>Không có bản ghi</span></td></tr>`;
                        $tableBody.html(html);
                        return false;
                    }

                    let startIndex = res.Body.Pagination.StartIndex || 1;
                    for (let item of data) {
                        //let htmlShow = item.Enable == 1 ? '<span class="text-center green"><i class="text-center green icon-status-ico-tsd"></i></span>' : '<span class="text-center red"><i class="text-center red icon-exit-ico-tsd"></i></span>';
                        html += `<tr class="TR_${item.Id}">` +
                            `<td class="text-center"><span>${startIndex}</span></td>` +
                            `<td class="text-center event-handle">` +
                            `<i class="icon-edit-ico-tsd btn-action btnEdit blue mr10" data-id="${item.Id}" title="Sửa"></i>` +
                            `<i class="icon-delete-ico-tsd btn-action btnDelete red" data-id="${item.Id}" title="Xóa"></i>` +
                            `</td>` +
                            `<td class="">${item.SKU}</td>` +
                            `<td class="">${item.Name}</td>` +
                            `<td class="text-end">${item.Price || ''}</td>` +
                            `<td class="text-end">${item.PriceDiscount || ''}</td>` +
                            `<td class="">${item.Quantity || ''}</td>` +
                            `<td class="">${item.CategoryName || ''}</td>` +
                            //`<td class="text-center">${htmlShow}</td>` +
                            `</tr>`;
                        startIndex++;
                    }
                    $tableBody.html(html);

                    $pagination.Set($footer, res.Body.Pagination, $page.GetList);

                    $page.ViewClick();
                    $page.DeleteClick();
                }
                else {

                }
            })
        },
        ViewClick: () => {
            $page.Self.find('.btnEdit').off('click').on('click', function () {
                let id = $(this).data('id');

                var getResponse = AjaxConfigHelper.SendRequestToServer(`/${$router}/GetItem`, "GET", { 'id': id });
                getResponse.then((res) => {
                    if (res.IsOk) {
                        let data = res.Body.Data.ProductItem || {};
                        for (let prop in data) {
                            if (prop == 'Enable') {
                                $modal.find(`[name=${prop}]`).prop('checked', data[prop] == 1);
                                continue;
                            }
                            if (prop.indexOf('Time') > -1 || prop.indexOf('Ngay') > -1) {
                                data[prop] = formatDateFrom_StringServer(data[prop]);
                            }
                            $modal.find(`[name=${prop}]`).val(data[prop]);
                        }

                        let dataVariant = res.Body.Data.Variants || [];
                        let htmlVariant = '';
                        for (let i = 0; i < dataVariant.length; i++) {
                            let item = dataVariant[i];
                            htmlVariant += `<tr data-id="${item.Id}">
                                                <td class="text-center">${(i + 1)}</td>
                                                <td><input class="w-100" name="variant-sku" value="${item.Sku}" readonly /></td>
                                                <td><input class="w-100 text-right" name="variant-stock" value="${item.Stock}" readonly /></td>
                                                <td class="text-center">
                                                    <i class="icon-edit-ico-tsd btn-action btnEditVariant blue mr10" data-id="${item.Id}" title="Sửa"></i>
                                                    <i class="icon-save-ico-tsd btn-action btnSaveVariant green" data-id="${item.Id}" title="Lưu"></i>
                                                </td>
                                            </tr>`
                        }
                        $modal.find('.tblVariant tbody').html(htmlVariant);
                        $page.EditVariant();

                        $img.drawTable(res.Body.Data.Images);
                        $modal.modal('show');
                    }
                    else {

                    }
                })
            })
        },
        EditVariant: () => {
            $modal.find('.tblVariant tbody').find('.btnEditVariant').on('click', function () {
                let $row = $(this).closest('tr');
                $row.find('input').prop('readonly', false);
            });
            $modal.find('.tblVariant tbody').find('.btnSaveVariant').on('click', function () {
                let $row = $(this).closest('tr');
                let id = $(this).data('id'), sku = $row.find('[name="variant-sku"]').val().trim(), stock = $row.find('[name="variant-stock"]').val().trim();

                var getResponse = AjaxConfigHelper.SendRequestToServer(`/${$router}/UpdateVariant`, "POST", { 'id': id, 'sku': sku, 'stock': stock });
                getResponse.then((res) => {
                    if (res.IsOk) {
                        $row.find('input').prop('readonly', true);
                    }
                    else {

                    }
                })


            })
        },
        DeleteClick: () => {
            $page.Self.find('.btnDelete').off('click').on('click', function () {
                let id = $(this).data('id');
                ConfirmDelete(function () {
                    var getResponse = AjaxConfigHelper.SendRequestToServer(`/${$router}/Delete`, "POST", { 'id': id });
                    getResponse.then((res) => {
                        if (res.IsOk) {
                            ToastSuccess("Xóa thành công");
                            $page.GetList();
                        }
                        else {

                        }
                    })
                })

            })
        },

    };

    var $ChiTiet = {
        init: function () {
            this.Save();
            this.ResetForm();
        },
        Self: $('#CHITIET'),
        ResetForm: () => {
            $modal.on('hidden.modal.bs', () => {
                $modal.find('input').val('').trigger('change');
                $form.find('[name=Id]').val(0);
                $modal.find('input:checkbox').prop('checked', true);
                $modal.find('select').find('option:first-child').prop('selected', true);
                $modal.validate().resetForm();
                $modal.find('.error').removeClass('error');
                $modal.find('.tblVariant tbody').html(`<tr><td class="text-center" colspan="5">Không có dữ liệu</td></tr>`);
                $img.table.find('tbody').html('');
            })
        },
        GetDataInput: () => {
            if (!$form.valid()) {
                return false;
            }
            let formData = new FormData();

            let data = GetFormDataToObject($form);
            formData.append('product', JSON.stringify(data));

            let attrValue = [];
            $modal.find('#attr select.product-attr').each(function (i, e) {
                let attr = $(e).val();
                if (attr && attr != '') {
                    let value = $(e).closest('.row').find('.attr-value').val().trim();
                    if (value != '') {
                        attrValue.push({ name: attr, value: value });
                    }
                }
            })

            formData.append('attrs', JSON.stringify(attrValue));

            //var files = $modal.find("#fileUpload #fileUpload-1")[0].files;
            for (var i = 0; i < files.length; i++) {
                formData.append("images", files[i]); // "images" phải trùng tên với param ở Controller
            }

            return formData;
        },
        Save: () => {
            $modal.find('#btn-save').off('click').on('click', () => {
                let formData = $ChiTiet.GetDataInput();
                if (!formData)
                    return false;
                let id = $form.find('[name=Id]').val();
                let action = id > 0 ? 'Update' : 'Create';

                $.ajax({
                    url: `/${$router}/${action}`,
                    type: 'POST',
                    data: formData,
                    contentType: false,
                    processData: false,
                    success: function (res) {
                        if (res.IsOk) {
                            let actionSub = id > 0 ? 'Cập nhật thành công' : 'Thêm mới thành công';
                            ToastSuccess(actionSub);
                            $page.GetList(id > 0 ? null : 1);
                            $modal.modal('hide');
                        }
                        else {
                            alert("Lỗi khi lưu sản phẩm!");
                        }
                    },
                    error: function (err) {
                        alert("Lỗi khi lưu sản phẩm!");
                    }
                });

                //var getResponse = AjaxConfigHelper.SendRequestFileToServer(`/${$router}/${action}`, "POST", formData);
                //getResponse.then((res) => {
                //    if (res.IsOk) {
                //        let actionSub = data.Id > 0 ? 'Cập nhật thành công' : 'Thêm mới thành công';
                //        ToastSuccess(actionSub);
                //        $page.GetList(data.Id > 0 ? null : 1);
                //        $modal.modal('hide');
                //    }
                //    else {
                //        /*ToastError(actionSub);*/
                //    }
                //})
            })
        }
    };

    var files = []; 
    var $img = {
        init: function () {
            $img.onChange();
        },
        table: $modal.find('#tblImg'),
        drawTable: function (images) {
            $.each(images, function (index, item) {
                var fileName = item.ImageName;
                var fileSize = "... KB";
                var preview = `<img src="${item.ImageUrl}" alt="${fileName}" height="30">`;

                $img.table.find('tbody').append(`
                            <tr data-id="${item.Id}">
                                <td class="text-center">${index + 1}</td>
                                <td>${fileName}</td>
                                <td class="text-center">${preview}</td>
                                <td>${fileSize}</td>
                                <td class="text-center"><button type="button" class="btn btn-delete" data-id="${item.Id}" title="Xóa"><i class="far fa-trash red"></i></button></td>
                            </tr>
                        `);
            });
            $img.deleteImg();
            $img.reindexTable();
        },
        deleteImg: function () {
            $img.table.find('tbody tr .btn-delete').off('click').on('click', function () {
                let id = $(this).data('id');
                let $tr = $(this).closest('tr');
                if (!id) {

                    const index = $(this).data("index");
                    files.splice(index, 1); // xóa file khỏi mảng
                    // Tạo lại FileList mới từ selectedFiles
                    const dt = new DataTransfer();
                    files.forEach(f => dt.items.add(f));
                    $("#fileUpload-1")[0].files = dt.files;
                    $("#fileUpload-1").trigger('change');
                    return false;
                }
                var getResponse = AjaxConfigHelper.SendRequestToServer(`/${$router}/RemoveImg`, "POST", { 'id': id });
                getResponse.then((res) => {
                    if (res.IsOk) {
                        $tr.remove();
                    }
                    else {

                    }
                })
            })
        },
        onChange: function () {
            $modal.find("#fileUpload-1").on('change', function () {
                $img.removeCache();
                files = Array.from(this.files);
                $.each(files, function (index, file) {
                    var fileName = file.name;
                    var fileSize = (file.size / 1024).toFixed(2) + " KB";
                    var fileType = file.type;
                    var preview = fileType.startsWith("image")
                        ? `<img src="${URL.createObjectURL(file)}" alt="${fileName}" height="30">`
                        : `<i class="material-icons-outlined">visibility_off</i>`;

                    $img.table.find('tbody').append(`
                            <tr>
                                <td class="text-center">${index + 1}</td>
                                <td>${fileName}</td>
                                <td class="text-center">${preview}</td>
                                <td>${fileSize}</td>
                                <td class="text-center"><button type="button" class="btn btn-delete" data-index="${index}" title="Xóa"><i class="far fa-trash red"></i></button></td>
                            </tr>
                        `);
                });
                $img.deleteImg();
                $img.reindexTable();
            })
        },
        removeCache: function () {
            $img.table.find('tbody tr').each(function (i, e) {
                let id = $(e).data('id');
                if (!id) {

                    $(e).remove();
                }
            })
        },
        reindexTable: function () {
            $img.table.find('tbody tr').each(function (i, e) {
                $(e).find('td').eq(0).html(i + 1);
            })
        }
    }

    $page.init();
    $ChiTiet.init();
    $img.init();
});