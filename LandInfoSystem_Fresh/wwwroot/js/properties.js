// =====================
// LOAD PROPERTIES
// =====================

async function loadProperties() {
  try {
    const response = await fetch('/api/landproperties', {
      method: 'GET',
      headers: {
        'Content-Type': 'application/json',
        'Accept': 'application/json'
      }
    });

    if (!response.ok) {
      throw new Error(`HTTP error! status: ${response.status}`);
    }

    let properties = await response.json();
    if (properties && properties.value && Array.isArray(properties.value)) {
        properties = properties.value;
    }
    const container = document.getElementById('properties-container');
    if (!container) return;

    container.innerHTML = '';

    if (!properties || properties.length === 0) {
      container.innerHTML = '<div class="loading">No properties available yet. Check back soon!</div>';
      return;
    }

    properties.forEach((prop, index) => {
      const card = createPropertyCard(prop, index);
      container.appendChild(card);
    });

  } catch (error) {
    console.error('Error loading properties:', error);
    const container = document.getElementById('properties-container');
    if (container) {
      container.innerHTML = '<div class="loading" style="color: #f44336;">Error loading properties.</div>';
    }
  }
}

// =====================
// CREATE PROPERTY CARD
// =====================

function createPropertyCard(property, index) {
  const card = document.createElement('div');
  card.className = 'property-card';
  card.style.position = 'relative';

  const price = formatCurrency(property.price || 0);
  const area = property.area || 0;
  const city = property.city || 'Unknown';
  const location = property.location || 'Unknown';
  const title = property.title || 'Property';

  // Verification Badge
  let verifiedBadge = '';
  if (property.documents && property.documents.some(d => d.status === 'Verified')) {
      verifiedBadge = '<div class="verified-property-badge"><i class="fas fa-check-circle"></i> Verified</div>';
  }

  // Determine image based on propertyId or fallback
  let imgSrc = `/images/land${(property.propertyId % 3) + 1}.png`; 
  if (property.imageUrl && !property.imageUrl.includes('no-image')) {
      if (property.imageUrl.startsWith('http')) {
          imgSrc = property.imageUrl;
      } else if (property.imageUrl.startsWith('/images/') || property.imageUrl.startsWith('images/')) {
          imgSrc = (property.imageUrl.startsWith('/') ? '' : '/') + property.imageUrl;
      } else {
          imgSrc = '/images/' + property.imageUrl.replace(/^\//, '');
      }
  }

  card.innerHTML = `
    ${verifiedBadge}
    <div class="property-image">
      <img src="${imgSrc}" alt="${title}" onerror="this.src='/images/land${(property.propertyId % 3) + 1}.png'; this.onerror=null;">
    </div>
    <div class="property-info">
      <div class="property-title">${title}</div>
      <div class="property-location">📍 ${location}</div>
      <div class="property-price">${price}</div>
      <div class="property-details">
        <span>📐 ${area.toLocaleString()} sq.ft</span>
        <span>🏙️ ${city}</span>
      </div>
      <div style="display:flex; gap:10px; margin-top:10px;">
        <button class="btn-primary" onclick="showPropertyDetails(${property.propertyId}); return false;" style="flex:1;">Details</button>
        <button class="btn-secondary" onclick="contactProperty(${property.propertyId}); return false;" style="flex:1;">Contact</button>
      </div>
    </div>
  `;

  return card;
}

function showPropertyDetails(id) {
    window.location.href = `property-details.html?id=${id}`;
}

// =====================
// CONTACT MODAL LOGIC
// =====================

let currentPropertyForContact = null;

async function contactProperty(propertyId) {
  const loggedInUser = JSON.parse(localStorage.getItem('user'));
  if (!loggedInUser) {
    alert("Please login first to contact the seller.");
    if (typeof show === 'function') show('auth');
    return;
  }

  try {
    const res = await fetch('/api/landproperties/' + propertyId);
    if (!res.ok) throw new Error("Property not found");
    const property = await res.json();
    currentPropertyForContact = property;

    const modalTitle = document.getElementById('modalPropTitle');
    const modalLoc = document.getElementById('modalPropLocation');
    const modalPrice = document.getElementById('modalPropPrice');
    const modalSeller = document.getElementById('modalSellerName');
    const modalEmail = document.getElementById('modalSellerEmail');
    const modalCall = document.getElementById('modalCallBtn');

    if(modalTitle) modalTitle.textContent = property.title;
    if(modalLoc) modalLoc.textContent = property.location + ", " + property.city;
    if(modalPrice) modalPrice.textContent = formatCurrency(property.price);
    
    const seller = property.seller || {};
    const sellerName = (seller.firstName || '') + ' ' + (seller.lastName || seller.username || 'Seller');
    if(modalSeller) modalSeller.textContent = sellerName;
    if(modalEmail) modalEmail.textContent = seller.email || '';
    
    if (modalCall) {
        if (seller.phone) {
            modalCall.href = "tel:" + seller.phone;
            modalCall.style.display = 'inline-block';
        } else {
            modalCall.style.display = 'none';
        }
    }

    document.getElementById('contactName').value = (loggedInUser.firstName || '') + ' ' + (loggedInUser.lastName || '');
    document.getElementById('contactEmail').value = loggedInUser.email || '';
    document.getElementById('contactPhone').value = loggedInUser.phone || '';

    const modal = document.getElementById('contactModal');
    if (modal) modal.style.display = 'flex';
    else console.warn("Contact modal element not found in DOM");
    
  } catch (err) {
    console.error(err);
    alert("Error loading seller details.");
  }
}

function closeContactModal() {
  document.getElementById('contactModal').style.display = 'none';
}

async function submitContactForm(event) {
  event.preventDefault();
  if (!currentPropertyForContact) return;

  const payload = {
    propertyId: currentPropertyForContact.propertyId,
    sellerId: currentPropertyForContact.sellerId,
    buyerName: document.getElementById('contactName').value,
    buyerEmail: document.getElementById('contactEmail').value,
    buyerPhone: document.getElementById('contactPhone').value,
    messageText: document.getElementById('contactMessage').value,
    createdDate: new Date().toISOString()
  };

  try {
    const res = await fetch('/api/messages', {
      method: 'POST',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify(payload)
    });

    if (res.ok) {
      alert("✅ Your message has been sent to the seller.");
      closeContactModal();
    } else {
      alert("❌ Failed to send message.");
    }
  } catch (err) {
    console.error(err);
    alert("Error connecting to server.");
  }
}

// =====================
// SELLER DASHBOARD LOGIC
// =====================

async function loadSellerProperties() {
  const loggedInUser = JSON.parse(localStorage.getItem('user'));
  if (!loggedInUser) return;

  try {
    const response = await fetch('/api/landproperties', {
      method: 'GET',
      headers: { 'Accept': 'application/json' }
    });

    if (!response.ok) throw new Error('Failed to load properties');

    let allProperties = await response.json();
    if (allProperties && allProperties.value && Array.isArray(allProperties.value)) {
        allProperties = allProperties.value;
    }
    const container = document.getElementById('sellerProperties');
    if (!container) return;

    container.innerHTML = '';
    const sellerProperties = allProperties.filter(p => p.sellerId == loggedInUser.userId);

    if (sellerProperties.length === 0) {
      container.innerHTML = '<div class="loading">No properties posted yet.</div>';
      return;
    }

    sellerProperties.forEach((prop) => {
      const card = createPropertyCard(prop);
      container.appendChild(card);
    });

  } catch (error) {
    console.error('Error loading seller properties:', error);
    const container = document.getElementById('sellerProperties');
    if (container) {
      container.innerHTML = `<div style="color:red; padding:20px;">Error loading seller properties: ${error.message}</div>`;
    }
  }
}

async function loadSellerMessages() {
  const seller = JSON.parse(localStorage.getItem('user'));
  if (!seller || seller.userType !== 'Seller') return;

  const container = document.getElementById('messagesList');
  if (!container) return;

  try {
    const res = await fetch('/api/messages/seller/' + seller.userId);
    let messages = await res.json();
    if (messages && messages.value && Array.isArray(messages.value)) {
        messages = messages.value;
    }

    if (messages.length === 0) {
      container.innerHTML = '<div class="loading">No inquiries received yet.</div>';
      return;
    }

    container.innerHTML = messages.map(msg => `
      <div class="message-item">
        <div class="message-header">
          <div class="buyer-info">
            <h4>${msg.buyerName}</h4>
            <p>${msg.buyerEmail} | ${msg.buyerPhone}</p>
          </div>
          <div class="message-date">${new Date(msg.createdDate).toLocaleDateString()}</div>
        </div>
        <div class="message-body">
          ${msg.messageText}
        </div>
      </div>
    `).join('');
  } catch (err) {
    console.error(err);
  }
}

// =====================
// PROPERTY POSTING LOGIC
// =====================

document.addEventListener('DOMContentLoaded', () => {
    const addPropForm = document.getElementById('addPropForm');
    if (addPropForm) {
        addPropForm.addEventListener('submit', handlePostProperty);
    }
});

async function handlePostProperty(e) {
    e.preventDefault();
    const loggedInUser = JSON.parse(localStorage.getItem('user'));
    if (!loggedInUser) return;

    const payload = {
        sellerId: loggedInUser.userId,
        title: document.getElementById('propTitle').value,
        area: parseFloat(document.getElementById('propArea').value),
        price: parseFloat(document.getElementById('propPrice').value),
        location: document.getElementById('propLocation').value,
        city: document.getElementById('propCity').value,
        state: document.getElementById('propState').value,
        pinCode: document.getElementById('propPinCode').value,
        description: document.getElementById('propDesc').value,
        isAvailable: true,
        createdDate: new Date().toISOString()
    };

    try {
        const response = await fetch('/api/landproperties', {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify(payload)
        });

        if (response.ok) {
            const newProp = await response.json();
            // Upload documents
            await uploadLandDocuments(newProp.propertyId, loggedInUser.userId);
            
            alert('✅ Property posted successfully! Documents uploaded and pending verification.');
            document.getElementById('addPropForm').reset();
            if (typeof togglePropertyForm === 'function') togglePropertyForm();
            loadSellerProperties();
            loadProperties();
        } else {
            alert('❌ Failed to post property.');
        }
    } catch (err) {
        console.error(err);
        alert('Error connecting to server.');
    }
}

async function uploadLandDocuments(propertyId, sellerId) {
    const docs = [
        { type: 'Sale Deed', file: document.getElementById('docSaleDeed').files[0] },
        { type: 'Patta Document', file: document.getElementById('docPatta').files[0] },
        { type: 'Ownership Proof', file: document.getElementById('docOwnership').files[0] },
        { type: 'EC Certificate', file: document.getElementById('docEC').files[0] }
    ];

    for (const doc of docs) {
        if (doc.file) {
            const formData = new FormData();
            formData.append('file', doc.file);
            formData.append('propertyId', propertyId);
            formData.append('sellerId', sellerId);
            formData.append('documentType', doc.type);

            try {
                await fetch('/api/documents/upload', {
                    method: 'POST',
                    body: formData
                });
            } catch (err) {
                console.error("Upload error:", err);
            }
        }
    }
}

// =====================
// ADMIN VERIFICATION LOGIC
// =====================

async function loadPendingVerifications() {
    const container = document.getElementById('verificationList');
    if (!container) return;

    try {
        const res = await fetch('/api/documents/pending');
        let docs = await res.json();
        if (docs && docs.value && Array.isArray(docs.value)) {
            docs = docs.value;
        }

        if (docs.length === 0) {
            container.innerHTML = '<div class="loading">No pending verification requests.</div>';
            return;
        }

        container.innerHTML = `
            <table class="admin-table-content">
                <thead>
                    <tr>
                        <th>Property</th>
                        <th>Type</th>
                        <th>Actions</th>
                    </tr>
                </thead>
                <tbody>
                    ${docs.map(doc => `
                        <tr>
                            <td>${doc.property ? doc.property.title : 'N/A'}</td>
                            <td>${doc.documentType}</td>
                            <td>
                                <div style="display:flex; gap:10px;">
                                    <button class="btn-primary" onclick="window.open('${doc.filePath}', '_blank')" style="padding:4px 8px; font-size:11px;">View</button>
                                    <button class="btn-primary" onclick="verifyDoc(${doc.id}, 'Verified')" style="padding:4px 8px; font-size:11px; background:#10b981;">Approve</button>
                                    <button class="btn-reset" onclick="rejectDoc(${doc.id})" style="padding:4px 8px; font-size:11px;">Reject</button>
                                </div>
                            </td>
                        </tr>
                    `).join('')}
                </tbody>
            </table>
        `;
    } catch (err) {
        console.error(err);
        container.innerHTML = '<div class="loading" style="color:#ef4444;">Error loading requests.</div>';
    }
}

async function verifyDoc(id, status, reason = "") {
    try {
        const res = await fetch('/api/documents/verify/' + id, {
            method: 'PUT',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify({ status, rejectionReason: reason })
        });

        if (res.ok) {
            alert("Document " + status + " successfully!");
            loadPendingVerifications();
        }
    } catch (err) {
        console.error(err);
    }
}

function rejectDoc(id) {
    const reason = prompt("Enter rejection reason:");
    if (reason) verifyDoc(id, 'Rejected', reason);
}

// =====================
// SEARCH & UTIL
// =====================

function searchProperties() {
  const city = document.getElementById('searchCity').value.toLowerCase();
  const minPrice = parseFloat(document.getElementById('minPrice').value) || 0;
  const maxPrice = parseFloat(document.getElementById('maxPrice').value) || Infinity;

  const container = document.getElementById('properties-container');
  const cards = container.querySelectorAll('.property-card');
  let found = 0;

  cards.forEach(card => {
    const title = card.querySelector('.property-title').textContent.toLowerCase();
    const location = card.querySelector('.property-location').textContent.toLowerCase();
    const priceText = card.querySelector('.property-price').textContent;
    const price = parseInt(priceText.replace(/[^0-9]/g, '')) || 0;

    const matchesCity = !city || location.includes(city) || title.includes(city);
    const matchesPrice = price >= minPrice && price <= maxPrice;

    if (matchesCity && matchesPrice) {
      card.style.display = 'block';
      found++;
    } else {
      card.style.display = 'none';
    }
  });

  if (found === 0) showMessage('❌ No properties found', 'error');
  else showMessage(`✅ Found ${found} properties`, 'success');
}

function resetSearch() {
  document.getElementById('searchCity').value = '';
  document.getElementById('minPrice').value = '';
  document.getElementById('maxPrice').value = '';
  loadProperties();
}

function formatCurrency(amount) {
  return new Intl.NumberFormat('en-IN', {
    style: 'currency',
    currency: 'INR',
    minimumFractionDigits: 0
  }).format(amount);
}

// GLOBAL EXPORTS
window.loadProperties = loadProperties;
window.loadSellerProperties = loadSellerProperties;
window.loadSellerMessages = loadSellerMessages;
window.searchProperties = searchProperties;
window.resetSearch = resetSearch;
window.contactProperty = contactProperty;
window.closeContactModal = closeContactModal;
window.submitContactForm = submitContactForm;
window.loadPendingVerifications = loadPendingVerifications;
window.verifyDoc = verifyDoc;
window.rejectDoc = rejectDoc;
window.showPropertyDetails = showPropertyDetails;
