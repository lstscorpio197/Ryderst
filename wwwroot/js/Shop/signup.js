const $form = $('#signup-form');

formFn = {
    init: function () {
        formFn.initValidate();
        formFn.signup();
    },
    initValidate: function () {
        $form.validate({
            rules: {
                name: {
                    required: true
                },
                password: {
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
                password: {
                    required: 'Vui lòng nhập mật khẩu'
                },
                email: {
                    required: 'Vui lòng nhập email',
                }
            }
        })
    },

    signup: function () {
        $form.find('#btnSignup').off('click').on('click', function (e) {
            if (!$form.valid()) {
                return false;
            }
            let data = GetFormDataToObject($form);
            if (!data)
                return false;

            $.blockUI({ message: null });

            var getResponse = AjaxConfigHelper.SendRequestToServer(`/Customer/Signup`, "POST", data);
            getResponse.then((res) => {
                if (res.IsOk) {
                    $.unblockUI({ message: null });
                    //toastSuccess(res.Body.Description);
                    formFn.drawSuccess();
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
    drawSuccess: function () {
        let html = `<div class="text-center d-flex flex-column" style="gap:15px">
                <i class="fas fa-circle-check green" style="font-size: 50px; color: green;"></i>
                <span>Đăng ký thành công</span>
                <div class="text-center d-flex flex-column">
                    <span>Bạn sẽ được chuyển tới trang đăng nhập sau <span class="countdown-time">5</span>s</span>
                    <a href="/signin">Đăng nhập ngay</a>
                </div>
            </div>`;
        $form.html(html);

        let countdown = 5;
        let interval = setInterval(() => {
            countdown--;
            $form.find('.countdown-time').text(countdown);
            if (countdown <= 0) {
                clearInterval(interval);
                window.location.href = '/signin';
            }
        }, 1000);
    }
}
formFn.init();