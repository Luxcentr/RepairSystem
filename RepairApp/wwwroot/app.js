const API_URL = '/api/repairs';

const STATUS_LABELS = {
    'New': 'Новый', 'InProgress': 'В ремонте',
    'WaitingParts': 'Ожидание запчастей', 'Done': 'Готово', 'Cancelled': 'Отменено'
};

const STATUS_CLASSES = {
    'New': 'status-new', 'InProgress': 'status-progress',
    'WaitingParts': 'status-waiting', 'Done': 'status-done', 'Cancelled': 'status-cancelled'
};

const PRIORITY_CLASSES = {
    'High': 'priority-high', 'Normal': 'priority-normal', 'Low': 'priority-low'
};

let currentFilter = 'all';

async function loadOrders() {
    try {
        const response = await fetch(API_URL);
        if (!response.ok) throw new Error('Ошибка загрузки');
        const orders = await response.json();
        renderOrders(orders);
        updateStats(orders);
    } catch (error) {
        console.error('Ошибка:', error);
        showToast('Не удалось загрузить заказы', 'error');
    }
}

function renderOrders(orders) {
    const tbody = document.getElementById('ordersTable');
    const emptyState = document.getElementById('emptyState');
    let filtered = currentFilter === 'all' ? orders : orders.filter(o => o.status === currentFilter);
    filtered.sort((a, b) => new Date(b.createdAt) - new Date(a.createdAt));

    if (filtered.length === 0) {
        tbody.innerHTML = '';
        emptyState.style.display = 'block';
        return;
    }
    emptyState.style.display = 'none';

    tbody.innerHTML = filtered.map(order => {
        const desc = order.description ? `<br><small style="color:#888;font-weight:normal;">${escapeHtml(order.description.substring(0, 40))}${order.description.length > 40 ? '...' : ''}</small>` : '';
        return `<tr>
            <td class="device-cell">${escapeHtml(order.deviceName)}${desc}</td>
            <td><div class="client-info"><span class="client-name">${escapeHtml(order.clientName)}</span>
                <span class="client-phone">${escapeHtml(order.phone || '')}</span></div></td>
            <td><span class="status ${STATUS_CLASSES[order.status] || ''}">
                <span class="status-dot"></span>${STATUS_LABELS[order.status] || order.status}</span></td>
            <td><span class="priority ${PRIORITY_CLASSES[order.priority] || ''}">
                ${translatePriority(order.priority)}</span></td>
            <td>${formatDate(order.createdAt)}</td>
            <td><div class="actions">${getActionButtons(order)}</div></td>
        </tr>`;
    }).join('');
}

function getActionButtons(order) {
    const buttons = [];
    if (order.status === 'New')
        buttons.push(`<button class="btn btn-warning" onclick="changeStatus('${order.id}', 'InProgress')">В ремонт</button>`);
    if (order.status === 'InProgress') {
        buttons.push(`<button class="btn btn-success" onclick="changeStatus('${order.id}', 'Done')">Готово</button>`);
        buttons.push(`<button class="btn btn-warning" onclick="changeStatus('${order.id}', 'WaitingParts')">Запчасти</button>`);
    }
    if (order.status === 'WaitingParts')
        buttons.push(`<button class="btn btn-success" onclick="changeStatus('${order.id}', 'InProgress')">В ремонт</button>`);
    if (order.status !== 'Done' && order.status !== 'Cancelled')
        buttons.push(`<button class="btn btn-danger" onclick="changeStatus('${order.id}', 'Cancelled')">Отмена</button>`);
    buttons.push(`<button class="btn btn-danger" onclick="deleteOrder('${order.id}', '${escapeHtml(order.deviceName)}')">Удалить</button>`);
    return buttons.join('');
}

async function createOrder(event) {
    event.preventDefault();
    const order = {
        deviceName: document.getElementById('deviceName').value.trim(),
        clientName: document.getElementById('clientName').value.trim(),
        phone: document.getElementById('phone').value.trim(),
        description: document.getElementById('description').value.trim(),
        priority: document.getElementById('priority').value,
        cost: parseFloat(document.getElementById('cost').value) || null
    };
    try {
        const response = await fetch(API_URL, {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify(order)
        });
        if (!response.ok) throw new Error('Ошибка');
        showToast('Заказ добавлен!', 'success');
        document.getElementById('orderForm').reset();
        await loadOrders();
    } catch (error) {
        showToast('Ошибка при добавлении', 'error');
    }
}

async function changeStatus(id, newStatus) {
    try {
        const response = await fetch(`${API_URL}/${id}/status`, {
            method: 'PUT',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify(newStatus)
        });
        if (!response.ok) throw new Error('Ошибка');
        showToast(`Статус: ${STATUS_LABELS[newStatus]}`, 'success');
        await loadOrders();
    } catch (error) {
        showToast('Не удалось изменить статус', 'error');
    }
}

async function deleteOrder(id, deviceName) {
    if (!confirm(`Удалить заказ на "${deviceName}"?`)) return;
    try {
        const response = await fetch(`${API_URL}/${id}`, { method: 'DELETE' });
        if (!response.ok) throw new Error('Ошибка');
        showToast('Заказ удален', 'success');
        await loadOrders();
    } catch (error) {
        showToast('Ошибка при удалении', 'error');
    }
}

function filterOrders(status) {
    currentFilter = status;
    document.querySelectorAll('.filter-btn').forEach(btn => btn.classList.remove('active'));
    event.target.classList.add('active');
    loadOrders();
}

function updateStats(orders) {
    document.getElementById('stat-total').textContent = orders.length;
    document.getElementById('stat-active').textContent = orders.filter(o =>
        o.status === 'New' || o.status === 'InProgress' || o.status === 'WaitingParts').length;
    document.getElementById('stat-done').textContent = orders.filter(o => o.status === 'Done').length;
}

function showToast(message, type) {
    const toast = document.getElementById('toast');
    toast.textContent = message;
    toast.className = `toast toast-${type} show`;
    setTimeout(() => toast.classList.remove('show'), 3000);
}

function escapeHtml(text) {
    if (!text) return '';
    const div = document.createElement('div');
    div.textContent = text;
    return div.innerHTML;
}

function formatDate(dateString) {
    return new Date(dateString).toLocaleDateString('ru-RU', {
        day: '2-digit', month: '2-digit', year: 'numeric', hour: '2-digit', minute: '2-digit'
    });
}

function translatePriority(p) {
    return { 'High': 'Срочно', 'Normal': 'Обычный', 'Low': 'Низкий' }[p] || p;
}

document.getElementById('orderForm').addEventListener('submit', createOrder);
loadOrders();
setInterval(loadOrders, 30000);