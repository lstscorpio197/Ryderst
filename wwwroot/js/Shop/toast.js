function toastSuccess(text) {
    $.toast({
        text: text,
        showHideTransition: 'plain',
        position: { left: 'auto', right: "20px", top: "80px", bottom: 'auto' },
        bgColor: '#42b85a',
        textColor: '#ffffff',
        loader: false,
    })
}
function toastWarning(text) {
    $.toast({
        text: text,
        showHideTransition: 'plain',
        position: { left: 'auto', right: "20px", top: "80px", bottom: 'auto' },
        bgColor: '#e4bd2f',
        textColor: '#ffffff',
        loader: false,
    })
}