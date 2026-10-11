document.addEventListener('DOMContentLoaded', function () {
    const raw = document.getElementById('compatData')?.textContent;
    if (!raw) return;

    let items = [];
    try {
        items = JSON.parse(raw);
    } catch (e) {
        items = [];
    }

    items.forEach(c => addCompatRow(c.brandId, c.modelId, c.engineId));
});