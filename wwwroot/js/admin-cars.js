let selectedBrandId = null;
let selectedModelId = null;
let selectedBrandName = '';
let selectedModelName = '';
let modalMode = '';
let modalType = '';

// ========================
// INIT — attach events after DOM ready
// ========================
document.addEventListener('DOMContentLoaded', function () {

    // Add Brand button
    document.getElementById('addBrandBtn')
        .addEventListener('click', () => openModal('brand'));

    // Add Model button
    document.getElementById('addModelBtn')
        .addEventListener('click', () => openModal('model'));

    // Add Engine button
    document.getElementById('addEngineBtn')
        .addEventListener('click', () => openModal('engine'));

    // Save button in modal
    document.getElementById('modalSaveBtn')
        .addEventListener('click', saveItem);

    // Enter key in modal
    document.getElementById('modalName')
        .addEventListener('keydown', function (e) {
            if (e.key === 'Enter') saveItem();
        });
});

// ========================
// MODAL
// ========================

function openModal(type) {
    modalMode = 'add';
    modalType = type;

    document.getElementById('modalItemId').value = 0;
    document.getElementById('modalType').value = type;
    document.getElementById('modalName').value = '';
    document.getElementById('modalError').textContent = '';

    const titles = {
        brand: 'إضافة ماركة جديدة',
        model: 'إضافة موديل لـ ' + selectedBrandName,
        engine: 'إضافة محرك لـ ' + selectedModelName
    };

    const placeholders = {
        brand: 'مثال: Toyota',
        model: 'مثال: Corolla',
        engine: 'مثال: 1.6'
    };

    document.getElementById('modalTitle').textContent = titles[type];
    document.getElementById('modalName').placeholder = placeholders[type];

    new bootstrap.Modal(document.getElementById('carsModal')).show();
    setTimeout(() => document.getElementById('modalName').focus(), 400);
}

function openEditModal(type, id, name) {
    modalMode = 'edit';
    modalType = type;

    document.getElementById('modalItemId').value = id;
    document.getElementById('modalType').value = type;
    document.getElementById('modalName').value = name;
    document.getElementById('modalError').textContent = '';

    const titles = {
        brand: 'تعديل الماركة',
        model: 'تعديل الموديل',
        engine: 'تعديل المحرك'
    };

    document.getElementById('modalTitle').textContent = titles[type];
    new bootstrap.Modal(document.getElementById('carsModal')).show();
    setTimeout(() => document.getElementById('modalName').focus(), 400);
}

async function saveItem() {
    const type = document.getElementById('modalType').value;
    const id = parseInt(document.getElementById('modalItemId').value);
    const name = document.getElementById('modalName').value.trim();

    if (!name) {
        document.getElementById('modalError').textContent = 'الاسم مطلوب';
        return;
    }

    document.getElementById('modalError').textContent = '';

    const params = new URLSearchParams();
    params.append('name', name);

    let url = '';

    if (type === 'brand') {
        if (modalMode === 'add') {
            url = '/Admin/Cars/CreateBrand';
        } else {
            url = '/Admin/Cars/EditBrand';
            params.append('id', id);
        }
    } else if (type === 'model') {
        if (modalMode === 'add') {
            url = '/Admin/Cars/CreateModel';
            params.append('carBrandId', selectedBrandId);
        } else {
            url = '/Admin/Cars/EditModel';
            params.append('id', id);
        }
    } else if (type === 'engine') {
        if (modalMode === 'add') {
            url = '/Admin/Cars/CreateEngine';
            params.append('carModelId', selectedModelId);
        } else {
            url = '/Admin/Cars/EditEngine';
            params.append('id', id);
        }
    }

    const res = await postForm(url, params);

    if (res.success) {
        bootstrap.Modal.getInstance(
            document.getElementById('carsModal')).hide();

        if (type === 'brand') {
            location.reload();
        } else if (type === 'model') {
            await loadModels(selectedBrandId, selectedBrandName);
        } else if (type === 'engine') {
            await loadEngines(selectedModelId, selectedModelName);
        }
    } else {
        document.getElementById('modalError').textContent = res.message;
    }
}

// ========================
// BRANDS
// ========================

async function selectBrand(brandId, brandName) {
    selectedBrandId = brandId;
    selectedBrandName = brandName;
    selectedModelId = null;
    selectedModelName = '';

    // Highlight selected
    document.querySelectorAll('#brandsList .cars-row')
        .forEach(el => el.classList.remove('selected'));
    const brandEl = document.getElementById('brand-' + brandId);
    if (brandEl) brandEl.classList.add('selected');

    // Reset engines column
    resetColumn('engines');

    // Load models
    await loadModels(brandId, brandName);
}

async function deleteItem(type, id) {
    const messages = {
        brand: 'هل أنت متأكد من حذف هذه الماركة؟',
        model: 'هل أنت متأكد من حذف هذا الموديل؟',
        engine: 'هل أنت متأكد من حذف هذا المحرك؟'
    };

    if (!confirm(messages[type])) return;

    const urls = {
        brand: '/Admin/Cars/DeleteBrand',
        model: '/Admin/Cars/DeleteModel',
        engine: '/Admin/Cars/DeleteEngine'
    };

    const params = new URLSearchParams();
    params.append('id', id);

    const res = await postForm(urls[type], params);

    if (res.success) {
        if (type === 'brand') {
            location.reload();
        } else if (type === 'model') {
            await loadModels(selectedBrandId, selectedBrandName);
            resetColumn('engines');
        } else if (type === 'engine') {
            await loadEngines(selectedModelId, selectedModelName);
        }
    } else {
        alert(res.message);
    }
}

// ========================
// MODELS
// ========================

async function loadModels(brandId, brandName) {
    document.getElementById('modelsTitle').textContent = 'موديلات ' + brandName;
    document.getElementById('addModelBtn').style.display = 'flex';

    const models = await fetchJson('/Admin/Cars/GetModels?brandId=' + brandId);

    const countEl = document.getElementById('modelsCount');
    countEl.textContent = models.length;
    countEl.style.display = 'inline-flex';

    const container = document.getElementById('modelsList');

    if (!models.length) {
        container.innerHTML = `
            <div class="cars-empty">
                <div class="cars-empty-icon">📋</div>
                <div>لا توجد موديلات — اضغط إضافة</div>
            </div>`;
        return;
    }

    container.innerHTML = models.map(m => `
        <div class="cars-row" id="model-${m.id}"
             onclick="selectModel(${m.id}, '${m.name}')">
            <div class="cars-row-content">
                <div class="cars-row-name">${m.name}</div>
                <div class="cars-row-sub">${m.enginesCount} محرك</div>
            </div>
            <div class="cars-row-actions">
                <button class="cars-action-btn edit"
                    onclick="event.stopPropagation();
                    openEditModal('model', ${m.id}, '${m.name}')">✏️</button>
                <button class="cars-action-btn delete"
                    onclick="event.stopPropagation();
                    deleteItem('model', ${m.id})">🗑️</button>
            </div>
            <span class="cars-row-arrow">›</span>
        </div>
    `).join('');
}

async function selectModel(modelId, modelName) {
    selectedModelId = modelId;
    selectedModelName = modelName;

    document.querySelectorAll('#modelsList .cars-row')
        .forEach(el => el.classList.remove('selected'));
    const modelEl = document.getElementById('model-' + modelId);
    if (modelEl) modelEl.classList.add('selected');

    await loadEngines(modelId, modelName);
}

// ========================
// ENGINES
// ========================

async function loadEngines(modelId, modelName) {
    document.getElementById('enginesTitle').textContent = 'محركات ' + modelName;
    document.getElementById('addEngineBtn').style.display = 'flex';

    const engines = await fetchJson('/Admin/Cars/GetEngines?modelId=' + modelId);

    const countEl = document.getElementById('enginesCount');
    countEl.textContent = engines.length;
    countEl.style.display = 'inline-flex';

    const container = document.getElementById('enginesList');

    if (!engines.length) {
        container.innerHTML = `
            <div class="cars-empty">
                <div class="cars-empty-icon">⚙️</div>
                <div>لا توجد محركات — اضغط إضافة</div>
            </div>`;
        return;
    }

    container.innerHTML = engines.map(e => `
        <div class="cars-row" id="engine-${e.id}">
            <div class="cars-row-content">
                <div class="cars-row-name">${e.name}</div>
            </div>
            <div class="cars-row-actions">
                <button class="cars-action-btn edit"
                    onclick="openEditModal('engine', ${e.id}, '${e.name}')">✏️</button>
                <button class="cars-action-btn delete"
                    onclick="deleteItem('engine', ${e.id})">🗑️</button>
            </div>
        </div>
    `).join('');
}

function resetColumn(col) {
    if (col === 'engines') {
        document.getElementById('enginesTitle').textContent = 'المحركات';
        document.getElementById('addEngineBtn').style.display = 'none';
        document.getElementById('enginesCount').style.display = 'none';
        document.getElementById('enginesList').innerHTML = `
            <div class="cars-empty">
                <div class="cars-empty-icon">👈</div>
                <div>اختر موديلاً أولاً</div>
            </div>`;
    }
}

// ========================
// HELPERS
// ========================

async function fetchJson(url) {
    const res = await fetch(url);
    return res.json();
}

async function postForm(url, params) {
    const token = document.querySelector(
        'input[name="__RequestVerificationToken"]')?.value;
    if (token) params.append('__RequestVerificationToken', token);

    const res = await fetch(url, {
        method: 'POST',
        headers: { 'Content-Type': 'application/x-www-form-urlencoded' },
        body: params.toString()
    });
    return res.json();
}