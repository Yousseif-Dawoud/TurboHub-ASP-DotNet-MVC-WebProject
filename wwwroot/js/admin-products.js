let compatRowCount = 0;
let brandsData = [];

// On page load, parse the brands data from the hidden element
document.addEventListener('DOMContentLoaded', function () {
    const raw = document.getElementById('brandsData')?.textContent;
    if (raw) {
        try {
            brandsData = JSON.parse(raw);
        } catch (e) {
            brandsData = [];
        }
    }
});

// ========================
// MAIN IMAGE PREVIEW
// ========================

// This function is called when the user selects a file from their device
function previewMainImage(input) {
    const preview = document.getElementById('mainImagePreview');
    if (input.files && input.files[0]) {
        const reader = new FileReader();
        reader.onload = e => {
            preview.innerHTML =
                `<img src="${e.target.result}"
                      style="width:100%;height:100%;object-fit:cover" />`;
        };
        reader.readAsDataURL(input.files[0]);
    }
}

// This function is called when the user enters an image URL
function previewMainUrl(url) {
    const preview = document.getElementById('mainImagePreview');
    if (url) {
        preview.innerHTML =
            `<img src="${url}" style="width:100%;height:100%;object-fit:cover"
                  onerror="this.parentElement.innerHTML='رابط غير صحيح'" />`;
    } else {
        preview.innerHTML = 'لا توجد صورة';
    }
}

// ========================
// EXTRA IMAGE URLS
// ========================

// This function adds a new input row for an extra image URL
function addExtraUrlRow() {
    const container = document.getElementById('extraUrlsList');
    const div = document.createElement('div');
    div.className = 'input-group mb-2';
    div.innerHTML = `
        <input type="text" name="ExtraImageUrls"
               class="form-control form-control-sm"
               placeholder="رابط صورة إضافية" />
        <button type="button"
                class="btn btn-outline-danger btn-sm"
                onclick="this.parentElement.remove()">×</button>
    `;
    container.appendChild(div);
}

// ========================
// COMPATIBLE CARS
// ========================

// This function adds a new compatibility row with brand/model/engine selectors
function addCompatRow(
    brandId = '', modelId = '', engineId = '',
    brandName = '', modelName = '', engineName = '') {

    const container = document.getElementById('compatList');
    const noMsg = document.getElementById('noCompatMsg');
    if (noMsg) noMsg.remove();

    const idx = compatRowCount++;
    const div = document.createElement('div');
    div.className = 'compat-row';
    div.id = `compat-${idx}`;

    const brandsOptions = brandsData.map(b =>
        `<option value="${b.id}"
            ${b.id == brandId ? 'selected' : ''}>${b.name}</option>`
    ).join('');

    div.innerHTML = `
        <div class="row g-2 align-items-end mb-2">
            <div class="col-md-3">
                <label class="form-label small">الماركة *</label>
                <select name="compatBrandIds"
                        class="form-select form-select-sm"
                        onchange="loadModelsForRow(this, ${idx})">
                    <option value="">— اختر —</option>
                    ${brandsOptions}
                </select>
            </div>
            <div class="col-md-3">
                <label class="form-label small">الموديل *</label>
                <select name="compatModelIds"
                        class="form-select form-select-sm"
                        id="models-${idx}"
                        onchange="loadEnginesForRow(this, ${idx})">
                    <option value="">— اختر الماركة أولاً —</option>
                </select>
            </div>
            <div class="col-md-3">
                <label class="form-label small">المحرك (اختياري)</label>
                <select name="compatEngineIds"
                        class="form-select form-select-sm"
                        id="engines-${idx}">
                    <option value="">— كل المحركات —</option>
                </select>
            </div>
            <div class="col-md-3">
                <button type="button"
                        class="btn btn-outline-danger btn-sm w-100"
                        onclick="removeCompatRow(${idx})">
                    حذف
                </button>
            </div>
        </div>
    `;

    container.appendChild(div);

    // If editing — preload models and engines
    if (brandId) {
        const brandSelect = div.querySelector('select[name="compatBrandIds"]');
        loadModelsForRow(brandSelect, idx, modelId, engineId);
    }
}


// This function removes a compatibility row by its index
function removeCompatRow(idx) {
    const row = document.getElementById(`compat-${idx}`);
    if (row) row.remove();

    const container = document.getElementById('compatList');
    if (!container.querySelector('.compat-row')) {
        const msg = document.createElement('div');
        msg.id = 'noCompatMsg';
        msg.className = 'text-muted small text-center py-3';
        msg.textContent = 'لم تُضف أي سيارة بعد — اضغط "إضافة سيارة"';
        container.appendChild(msg);
    }
}


// This function loads models for a given brand and updates the model select element
async function loadModelsForRow(
    brandSelect, idx, preselectedModelId = null, preselectedEngineId = null) {

    const brandId = brandSelect.value;
    const modelSelect = document.getElementById(`models-${idx}`);
    const engineSelect = document.getElementById(`engines-${idx}`);

    modelSelect.innerHTML =
        '<option value="">جاري التحميل...</option>';
    engineSelect.innerHTML =
        '<option value="">— كل المحركات —</option>';

    if (!brandId) {
        modelSelect.innerHTML =
            '<option value="">— اختر الماركة أولاً —</option>';
        return;
    }

    const models = await fetchJson(
        '/Admin/Products/GetModelsByBrand?brandId=' + brandId);

    modelSelect.innerHTML = '<option value="">— اختر الموديل —</option>' +
        models.map(m =>
            `<option value="${m.id}"
                ${m.id == preselectedModelId ? 'selected' : ''}>${m.name}</option>`
        ).join('');

    if (preselectedModelId) {
        await loadEnginesForRow(modelSelect, idx, preselectedEngineId);
    }
}


// This function loads engines for a given model and updates the engine select element
async function loadEnginesForRow(
    modelSelect, idx, preselectedEngineId = null) {

    const modelId = modelSelect.value;
    const engineSelect = document.getElementById(`engines-${idx}`);

    if (!modelId) {
        engineSelect.innerHTML =
            '<option value="">— كل المحركات —</option>';
        return;
    }

    const engines = await fetchJson(
        '/Admin/Products/GetEnginesByModel?modelId=' + modelId);

    engineSelect.innerHTML =
        '<option value="">— كل المحركات —</option>' +
        engines.map(e =>
            `<option value="${e.id}"
                ${e.id == preselectedEngineId ? 'selected' : ''}>${e.name}</option>`
        ).join('');
}


// This function fetches JSON data from a given URL
async function fetchJson(url) {
    const res = await fetch(url);
    return res.json();
}