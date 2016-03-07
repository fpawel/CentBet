function dateTimeToString(date) {
    var options = { year: "numeric", month: "long", day: "numeric", hour: "numeric", minute: "numeric", second: "numeric" };
    return (new Date(date)).toLocaleString("ru", options);
}

function createBlobFromString(string) {
    var blob = new Blob([string], {
        type: "plain/text",
        endings: "native"
    });
    return blob;
};