function validateUser() {
    if (confirm('Are you sure you want to delete this customer?')) {
        return true;
    }
    else {
        return false;
    }
}