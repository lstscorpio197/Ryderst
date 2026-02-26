const $form = $('#infor-form'); 

pageFn = {
    init: function () {
        pageFn.initValidate();
        pageFn.update();
    },
    initValidate: function () {
        $form.validate({
            rules: {
                name: {
                    required: true
                },
                phone: {
                    required: true,
                },
                address: {
                    required: true,
                },
                email: {
                    required: true,
                }
            },
            messages: {
                name: {
                    required: 'Vui lòng nhập họ tên'
                },
                phone: {
                    required: 'Vui lòng nhập số điện thoại'
                },
                address: {
                    required: 'Vui lòng nhập địa chỉ',
                },
                email: {
                    required: 'Vui lòng nhập email',
                }
            }
        })
    },

    update: function () {
        $form.find('#btnUpdate').off('click').on('click', function (e) {
            if (!$form.valid()) {
                return false;
            }
            let data = GetFormDataToObject($form);
            if (!data)
                return false;

            $.blockUI({ message: null });

            var getResponse = AjaxConfigHelper.SendRequestToServer(`/Customer/UpdateInfo`, "POST", data);
            getResponse.then((res) => {
                if (res.IsOk) {
                    $.unblockUI({ message: null });
                    toastSuccess(res.Body.Description);
                }
                else {
                    $.unblockUI({ message: null });
                    toastWarning(res.Body.Description);
                }
            }).catch((err) => {
                $.unblockUI({ message: null });
                toastError(err);
            });
        })
    },
}
pageFn.init();