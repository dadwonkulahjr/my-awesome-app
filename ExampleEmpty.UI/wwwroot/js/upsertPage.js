function validateInput() {
    var doc = document.getElementById('uploadBox');
    if (doc.value == '') {
        alert('Please select an image');
        return false;
    }
    return true;
}