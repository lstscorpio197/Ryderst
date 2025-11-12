var $header = $('.frame-search');
const $footer = $('.frame-footer');
const $table = $('#TBLDANHSACH');
const $tableBody = $table.find('tbody');
const $modal = $('#CHITIET');
const $router = "Admin/Collection";
const $form = $('#ModalForm');


function DataSearch(pageNum) {
    this.Ten = $header.find('[name=Ten]').val().trim();
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
                        required: "Tên bộ sưu tập không được để trống"
                    }
                }
            })
        },
        initGiaoDien: function () {
            $('#Description').summernote({
                height: 100,
                toolbar: [
                    // Customize the toolbar to remove image, link, video, and table options
                    ['style', ['bold', 'italic', 'underline', 'clear']],
                    ['font', ['strikethrough', 'superscript', 'subscript']],
                    ['para', ['ul', 'ol', 'paragraph']],
                    ['height', ['height']],
                    ['misc', ['fullscreen', 'codeview']]
                ],
                callbacks: {
                    onImageUpload: function (files) {
                        var data = new FormData();
                        data.append('file', files[0]);
                        $.ajax({
                            url: `/${$router}/UploadImage`,
                            method: 'POST',
                            data: data,
                            contentType: false,
                            processData: false,
                            success: function (url) {
                                $('#Description').summernote('insertImage', url);
                            }
                        });
                    }
                }
            });

            $modal.find('.btnChooseFile').off('click').on('click', function () {
                $modal.find('input[name="ThumbnailFile"]').trigger('click');
            })

            $modal.find('input[type="file"]').on('change', function (event) {
                $modal.find('input[name=Thumbnail]').val('');
                var files = event.target.files;
                if (files.length > 0) {
                    var reader = new FileReader();
                    reader.onload = function (e) {
                        $('#previewImage').attr('src', e.target.result).show();
                        $modal.find('#previewImage').attr('src', e.target.result);
                    };
                    reader.readAsDataURL(files[0]);
                } else {
                    $('#previewImage').attr('src', '').show();
                    $modal.find('#previewImage').attr('src', '');
                }
            });
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
                        html = `<tr><td class="text-center" colspan="4"><span>Không có bản ghi</span></td></tr>`;
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
                            `<td class=""><span>${item.Name}</span></td>` +
                            `<td class=""><span>${item.Slug}</span></td>` +
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
                        let data = res.Body.Data || {};
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
                        $('#Description').summernote('code', res.Body.Data.Description);

                        $('#previewImage').attr('src', res.Body.Data.Thumbnail).show();
                        $modal.find('#previewImage').attr('src', res.Body.Data.Thumbnail);

                        let productIds = res.Body.Data.Products || [];
                        for (let p of productIds) {
                            $modal.find('#tblProduct').find(`[type=checkbox][data-id="${p.Id}"]`).prop('checked', true).trigger('change');
                        }
                        $modal.modal('show');
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
            this.initProductTable();
        },
        Self: $('#CHITIET'),
        ResetForm: () => {
            $modal.on('hidden.modal.bs', () => {
                $form.find('input').val('');
                $form.find('[name=Id]').val(0);
                $form.find('input:checkbox').prop('checked', true);
                $form.find('select').find('option:first-child').prop('selected', true);
                $('#Description').summernote('code', '');
                $form.validate().resetForm();
                $form.find('.error').removeClass('error');

                $modal.find('#tblProduct').find('[type=checkbox]').prop('checked', false);
                $modal.find('#previewImage').attr('src', '').hide();
            })
        },
        initProductTable: function () {
            var getResponse = AjaxConfigHelper.SendRequestToServer(`/${$router}/GetListProduct`, "GET", null);
            getResponse.then((res) => {
                if (res.IsOk) {
                    let data = res.Body.Data || [];

                    if (data.length == 0) {
                        html = `<tr><td class="text-center" colspan="4"><span>Không có bản ghi</span></td></tr>`;
                        $modal.find('#tblProduct').html(html);
                        return false;
                    }

                    let html = '';
                    let startIndex = 1;
                    for (let item of data) {
                        html += `<tr class="TR_${item.Id}">` +
                            `<td class="text-center"><input type="checkbox" data-id="${item.Id}" class="checkbox-item" /></td>` +
                            `<td class="text-center">${startIndex}</td>` +
                            `<td class="">${item.SKU}</td>` +
                            `<td class="">${item.Name}</td>` +
                            `</tr>`;
                        startIndex++;
                    }
                    $modal.find('#tblProduct tbody').html(html);
                    $ChiTiet.HandleCheckboxEvent();
                }
            })
        },
        HandleCheckboxEvent: function () {
            const $checkAll = $modal.find('.check-all');
            const $checkboxItems = $modal.find('.checkbox-item');

            $checkAll.on('change', function () {
                const isChecked = $(this).is(':checked');
                $checkboxItems.prop('checked', isChecked);
            });

            $checkboxItems.on('change', function () {
                const allChecked = $checkboxItems.length === $checkboxItems.filter(':checked').length;
                $checkAll.prop('checked', allChecked);
            });
        },
        GetSelectedProduct: function () {
            const selectedIds = [];
            $modal.find('#tblProduct tbody .checkbox-item:checked').each(function () {
                selectedIds.push($(this).data('id'));
            });
            return selectedIds;
        },
        GetDataInput: () => {
            if (!$form.valid()) {
                return false;
            }
            let data = GetFormDataToObject($form);
            data.Description = $('#Description').summernote('code');

            // Create a new FormData object
            let formData = new FormData();

            formData.append('data', JSON.stringify(data));
            formData.append('productIds', JSON.stringify($ChiTiet.GetSelectedProduct()));
            let thumbnailFile = $modal.find('input[name="ThumbnailFile"]').prop('files')[0];
            if (thumbnailFile) {
                formData.append('file', thumbnailFile); // Append the file
            }
            return formData; // Return the FormData object
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
                            alert("Lỗi khi lưu bộ sưu tập!");
                        }
                    },
                    error: function (err) {
                        alert("Lỗi khi lưu bộ sưu tập!");
                    }
                });
            })
        }
    };


    $page.init();
    $ChiTiet.init();
});