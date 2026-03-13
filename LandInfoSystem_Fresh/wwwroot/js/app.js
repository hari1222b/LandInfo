const API_URL = '/api';
let user = null;
let scene, camera, renderer;
let leafletMap = null;
let heatLayer = null;

// NAVIGATION
function navigateTo(section) {
  show(section);
}

// SHOW/HIDE SECTIONS
function show(sectionId) {
  document.querySelectorAll('.section').forEach(s => s.style.display = 'none');

  const el = document.getElementById(sectionId);
  if (el) el.style.display = 'block';

  if (sectionId === 'home') init3D();
  if (sectionId === 'properties') loadProperties();
  if (sectionId === 'map-section') initMap();
  if (sectionId === 'profile') renderProfile();

  // Role-Based UI Rendering for Dashboard
  if (sectionId === 'dashboard' && user) {
    const roleBadge = document.getElementById("dashboardRole");
    const sellerDash = document.getElementById("sellerDash");
    const buyerDash = document.getElementById("buyerDash");

    if (roleBadge) roleBadge.innerText = (user.userType || 'User') + " Account";

    if (user.userType === 'Seller') {
      if (sellerDash) sellerDash.style.display = "block";
      if (buyerDash) buyerDash.style.display = "none";
      loadSellerProperties();
      loadSellerMessages();
    } else {
      // Default to Buyer view if Buyer or unassigned
      if (sellerDash) sellerDash.style.display = "none";
      if (buyerDash) buyerDash.style.display = "block";
    }
  }

  window.scrollTo(0, 0);
}

// =====================
// INTERACTIVE MAP WITH HEATMAP
// =====================
function initMap() {
  const mapContainer = document.getElementById('propertyMap');
  if (!mapContainer || leafletMap) {
    if (leafletMap) leafletMap.invalidateSize();
    return;
  }

  // Initialize Map focused on India
  leafletMap = L.map('propertyMap').setView([21.5937, 78.9629], 5);

  // Premium Dark Tile Layer
  const darkTiles = L.tileLayer('https://{s}.basemaps.cartocdn.com/dark_all/{z}/{x}/{y}{r}.png', {
    attribution: '&copy; CartoDB',
    subdomains: 'abcd',
    maxZoom: 20
  });

  // Satellite Tile Layer
  const satelliteTiles = L.tileLayer('https://mt1.google.com/vt/lyrs=s&x={x}&y={y}&z={z}', {
      attribution: '&copy; Google Maps',
      maxZoom: 20
  });

  const baseMaps = {
      "Dark Mode": darkTiles,
      "Satellite": satelliteTiles
  };

  darkTiles.addTo(leafletMap);
  L.control.layers(baseMaps).addTo(leafletMap);

  // Generate Sample Heatmap Data (lat, lng, intensity)
  const addressPoints = [];
  const centers = [
    { lat: 19.0760, lng: 72.8777, spread: 2 },  // Mumbai
    { lat: 28.7041, lng: 77.1025, spread: 1.5 }, // Delhi
    { lat: 12.9716, lng: 77.5946, spread: 1.8 }, // Bangalore
    { lat: 17.3850, lng: 78.4867, spread: 1.5 }, // Hyderabad
    { lat: 13.0827, lng: 80.2707, spread: 1 }    // Chennai
  ];

  for (let center of centers) {
    for (let i = 0; i < 100; i++) {
      let lat = center.lat + (Math.random() - 0.5) * center.spread;
      let lng = center.lng + (Math.random() - 0.5) * center.spread;
      let intensity = Math.random() * 0.8 + 0.2; // 0.2 to 1.0
      addressPoints.push([lat, lng, intensity]);
    }
  }

  // Define Heatmap Layer with Gold/Red premium colors
  heatLayer = L.heatLayer(addressPoints, {
    radius: 25,
    blur: 15,
    maxZoom: 17,
    gradient: {0.4: 'yellow', 0.65: 'orange', 1: 'red'}
  });

  // Toggle button logic
  const toggleBtn = document.getElementById('toggleHeatmapBtn');
  let heatVisible = false;

  toggleBtn.addEventListener('click', () => {
    if (heatVisible) {
      leafletMap.removeLayer(heatLayer);
      toggleBtn.innerText = 'Toggle Heatmap';
    } else {
      leafletMap.addLayer(heatLayer);
      toggleBtn.innerText = 'Hide Heatmap';
    }
    heatVisible = !heatVisible;
  });

  // Add marker for demonstration
  L.marker([19.0760, 72.8777])
    .addTo(leafletMap)
    .bindPopup('<b>Mumbai Prime Property</b><br>Available now.')
    .openPopup();
}

// 3D SCENE
function init3D() {

const container = document.getElementById('canvas-3d');
if (!container || container.querySelector('canvas')) return;

try {

```
scene = new THREE.Scene();
scene.background = new THREE.Color(0x1a1a2e);

camera = new THREE.PerspectiveCamera(75, window.innerWidth/window.innerHeight,0.1,1000);
camera.position.set(0,5,15);

renderer = new THREE.WebGLRenderer({antialias:true});
renderer.setSize(window.innerWidth, window.innerHeight);

container.appendChild(renderer.domElement);

const light = new THREE.DirectionalLight(0xffffff,0.8);
light.position.set(20,30,20);
scene.add(light);

scene.add(new THREE.AmbientLight(0xffffff,0.4));

const colors=[0xffd700,0x667eea,0x4ecdc4,0xff6b6b,0x95e1d3,0xf38181];

for(let i=0;i<6;i++){

  const geometry=new THREE.BoxGeometry(5,5,5);
  const material=new THREE.MeshStandardMaterial({color:colors[i]});
  const mesh=new THREE.Mesh(geometry,material);

  mesh.position.set(i*8-20,2.5,0);
  scene.add(mesh);

}

function animate(){

  requestAnimationFrame(animate);

  scene.children.forEach(child=>{
    if(child.isMesh){
      child.rotation.y+=0.005;
    }
  });

  camera.position.x=Math.sin(Date.now()*0.0001)*20;
  camera.position.z=Math.cos(Date.now()*0.0001)*25;

  camera.lookAt(scene.position);

  renderer.render(scene,camera);

}

animate();
```

} catch(err){

```
console.error("3D Error",err);
```

}

}

// TOGGLE AUTH FORMS
function switchToRegister() {
  const loginBox = document.getElementById('loginBox');
  const registerBox = document.getElementById('registerBox');
  if (loginBox) loginBox.style.display = 'none';
  if (registerBox) registerBox.style.display = 'block';
}

function switchToLogin() {
  const registerBox = document.getElementById('registerBox');
  const loginBox = document.getElementById('loginBox');
  if (registerBox) registerBox.style.display = 'none';
  if (loginBox) loginBox.style.display = 'block';
}

function toggleAuthMode(mode) {
  if (mode === 'register') {
    switchToRegister();
  } else {
    switchToLogin();
  }
}

function toggleAdminSecret() {
  const type = document.getElementById('regUserType').value;
  const secretInput = document.getElementById('regAdminSecret');
  if (type === 'Admin') {
    secretInput.style.display = 'block';
    secretInput.setAttribute('required', 'true');
  } else {
    secretInput.style.display = 'none';
    secretInput.removeAttribute('required');
  }
}

// LOGOUT
function logout() {
  localStorage.removeItem("user");
  user = null;
  updateNav();
  show("home");
  showMessage("Logged out successfully", "success");
}

// NAV UPDATE
function updateNav() {
  const dashBtn = document.getElementById("navDashboard");
  const adminBtn = document.getElementById("navAdmin");
  const authBtn = document.getElementById("navAuth");
  const logoutBtn = document.getElementById("navLogout");
  const greeting = document.getElementById("userGreeting");
  const profileBtn = document.getElementById("navProfile");

  // Mobile Menu Elements
  const mobDash = document.getElementById("mobNavDashboard");
  const mobProfile = document.getElementById("mobNavProfile");
  const mobAuth = document.getElementById("mobNavAuth");
  const mobLogout = document.getElementById("mobNavLogout");
  const mobGreeting = document.getElementById("mobUserGreeting");

  // Hero section buttons
  const heroSignIn = document.getElementById("heroSignIn");
  const heroDash = document.getElementById("heroDash");

  // Conditional features (AI & Loan)
  const navAI = document.getElementById("navAI");
  const navLoan = document.getElementById("navLoan");
  const mobAI = document.getElementById("mobNavAI");
  const mobLoan = document.getElementById("mobNavLoan");

  if (user) {
    if (authBtn) authBtn.style.display = "none";
    if (mobAuth) mobAuth.style.display = "none";
    
    if (profileBtn) profileBtn.style.display = "block";
    if (mobProfile) mobProfile.style.display = "block";

    if (navAI) navAI.style.display = "block";
    if (navLoan) navLoan.style.display = "block";
    if (mobAI) mobAI.style.display = "block";
    if (mobLoan) mobLoan.style.display = "block";

    if (logoutBtn) {
      logoutBtn.style.display = "flex";
      logoutBtn.style.alignItems = "center";
    }
    if (mobLogout) mobLogout.style.display = "block";

    if (greeting) greeting.textContent = "Hi, " + (user.firstName || user.username);
    if (mobGreeting) mobGreeting.textContent = "Hi, " + (user.firstName || user.username);

    if (user.userType === 'Admin') {
      if (adminBtn) adminBtn.style.display = "block";
      if (dashBtn) dashBtn.style.display = "none";
      if (mobDash) mobDash.style.display = "none"; 
      
      if (heroDash) {
        heroDash.style.display = "block";
        heroDash.innerText = "Admin Panel";
        heroDash.onclick = () => show('admin');
      }
    } else {
      if (adminBtn) adminBtn.style.display = "none";
      if (dashBtn) dashBtn.style.display = "block";
      if (mobDash) mobDash.style.display = "block";

      if (heroDash) {
        heroDash.style.display = "block";
        heroDash.innerText = "My Dashboard";
        heroDash.onclick = () => show('dashboard');
      }
    }
    if (heroSignIn) heroSignIn.style.display = "none";
  } else {
    if (authBtn) authBtn.style.display = "block";
    if (mobAuth) mobAuth.style.display = "block";
    
    if (profileBtn) profileBtn.style.display = "none";
    if (mobProfile) mobProfile.style.display = "none";
    
    if (logoutBtn) logoutBtn.style.display = "none";
    if (mobLogout) mobLogout.style.display = "none";
    
    if (dashBtn) dashBtn.style.display = "none";
    if (mobDash) mobDash.style.display = "none";
    
    if (adminBtn) adminBtn.style.display = "none";
    
    if (navAI) navAI.style.display = "none";
    if (navLoan) navLoan.style.display = "none";
    if (mobAI) mobAI.style.display = "none";
    if (mobLoan) mobLoan.style.display = "none";

    if (heroSignIn) heroSignIn.style.display = "block";
    if (heroDash) heroDash.style.display = "none";
  }
}

// =====================
// DASHBOARD & ADMIN HANDLERS
// =====================
function togglePropertyForm() {
  const form = document.getElementById('propertyForm');
  if (form) {
    if (form.style.display === 'none' || form.style.display === '') {
      form.style.display = 'block';
    } else {
      form.style.display = 'none';
    }
  }
}

function showAdminTab(tabName) {
  // Hide all admin tabs
  const tabs = document.querySelectorAll('.admin-tab');
  tabs.forEach(tab => tab.style.display = 'none');
  
  // Remove active class from buttons
  const btns = document.querySelectorAll('.admin-tabs .tab-btn');
  btns.forEach(btn => btn.classList.remove('active'));

  // Show selected tab
  let targetTab = null;
  if (tabName === 'users') targetTab = document.getElementById('adminUsers');
  if (tabName === 'properties') targetTab = document.getElementById('adminProperties');
  if (tabName === 'analytics') targetTab = document.getElementById('adminAnalytics');
  if (tabName === 'verifications') {
      targetTab = document.getElementById('adminVerifications');
      loadPendingVerifications();
  }
  
  if (targetTab) targetTab.style.display = 'block';
  
  // Update active button (this relies on the element triggering the event or simple finding)
  const clickedBtn = Array.from(btns).find(b => b.textContent.toLowerCase().includes(tabName));
  if (clickedBtn) clickedBtn.classList.add('active');
}

// Properties logic handled in properties.js

// CHATBOT
function toggleChat(){
  const win = document.getElementById("chatWindow");
  if(win){
    win.style.display = (win.style.display === "none" || win.style.display === "") ? "flex" : "none";
  }
}

async function sendChat() {
  const input = document.getElementById("chatInput");
  const text = input.value.trim();
  if (!text) return;

  const box = document.getElementById("chatBox");

  // User Message
  const p = document.createElement("p");
  p.className = "user-msg";
  p.style.background = "var(--primary)";
  p.style.color = "white";
  p.style.padding = "8px 12px";
  p.style.borderRadius = "12px";
  p.style.alignSelf = "flex-end";
  p.style.marginBottom = "10px";
  p.style.maxWidth = "80%";
  p.textContent = text;
  box.appendChild(p);

  input.value = "";
  box.scrollTop = box.scrollHeight;

  try {
    const res = await fetch(API_URL + "/chatbot/chat", {
      method: "POST",
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify({ message: text })
    });

    const data = await res.json();
    
    const reply = document.createElement("p");
    reply.className = "ai-msg";
    reply.style.background = "var(--bg-card)";
    reply.style.color = "var(--text-main)";
    reply.style.border = "1px solid var(--border)";
    reply.style.padding = "8px 12px";
    reply.style.borderRadius = "12px";
    reply.style.alignSelf = "flex-start";
    reply.style.marginBottom = "10px";
    reply.style.maxWidth = "80%";
    
    reply.textContent = (data.message || "I'm having trouble understanding right now. Please try again later.");
    box.appendChild(reply);

  } catch (err) {
    console.error("Chatbot backend error:", err);
    const reply = document.createElement("p");
    reply.className = "ai-msg";
    reply.style.background = "var(--bg-card)";
    reply.style.color = "var(--text-main)";
    reply.style.border = "1px solid red";
    reply.style.padding = "8px 12px";
    reply.style.borderRadius = "12px";
    reply.style.alignSelf = "flex-start";
    reply.style.marginBottom = "10px";
    reply.style.maxWidth = "80%";
    reply.textContent = "Server connection error. Our AI is currently offline.";
    box.appendChild(reply);
  }
  
  box.scrollTop = box.scrollHeight;
}

// CLEAR CHAT
function clearChat() {
  const box = document.getElementById("chatBox");
  if (box) {
    box.innerHTML = '';
    const p = document.createElement("p");
    p.className = "ai-msg";
    p.style.background = "var(--bg-card)";
    p.style.color = "var(--text-main)";
    p.style.border = "1px solid var(--border)";
    p.style.padding = "8px 12px";
    p.style.borderRadius = "12px";
    p.style.alignSelf = "flex-start";
    p.style.marginBottom = "10px";
    p.style.maxWidth = "80%";
    p.textContent = "Chat cleared. How can I help you now?";
    box.appendChild(p);
  }
}

// VOICE RECOGNITION
function startVoiceRecognition() {
  const SpeechRecognition = window.SpeechRecognition || window.webkitSpeechRecognition;
  if (!SpeechRecognition) {
    msg("Voice typing not supported in this browser", "error");
    return;
  }

  const recognition = new SpeechRecognition();
  const voiceBtn = document.getElementById("voiceBtn");
  const chatInput = document.getElementById("chatInput");

  recognition.onstart = () => {
    voiceBtn.classList.add("listening");
    chatInput.value = ''; // Clear input as requested
    chatInput.placeholder = "Listening...";
  };

  recognition.onresult = (event) => {
    const transcript = event.results[0][0].transcript;
    chatInput.value = transcript;
    voiceBtn.classList.remove("listening");
    chatInput.placeholder = "Ask about properties, process, etc...";
    // Automatically send after a short pause if desired, or let user review
    setTimeout(() => sendChat(), 500);
  };

  recognition.onerror = () => {
    voiceBtn.classList.remove("listening");
    chatInput.placeholder = "Ask about properties, process, etc...";
    msg("Voice error. Try again.", "error");
  };

  recognition.onend = () => {
    voiceBtn.classList.remove("listening");
    chatInput.placeholder = "Ask about properties, process, etc...";
  };

  recognition.start();
}

// PROFILE RENDERING
function renderProfile() {
  if (!user) {
    show('auth');
    return;
  }

  const nameEl = document.getElementById("profName");
  const userEl = document.getElementById("profUsername");
  const emailEl = document.getElementById("profEmail");
  const typeEl = document.getElementById("profType");
  const badgeEl = document.getElementById("profTypeBadge");
  const dateEl = document.getElementById("profDate");

  if (nameEl) nameEl.textContent = (user.firstName || user.username) + "'s Profile";
  if (userEl) userEl.textContent = user.username;
  if (emailEl) emailEl.textContent = user.email || "No email provided";
  if (typeEl) typeEl.textContent = user.userType || "Buyer";
  if (badgeEl) badgeEl.textContent = user.userType || "Buyer";
  if (dateEl) dateEl.textContent = user.createdAt ? new Date(user.createdAt).toLocaleDateString() : "March 12, 2026";
}

// MESSAGE
function msg(text,type){
  const m = document.getElementById("message");
  if(!m) return;
  m.textContent = text;
  m.className = "message " + type + " show";
  setTimeout(() => m.classList.remove("show"), 3000);
}

// INIT
window.addEventListener("load", () => {
  console.log("App Started");

  const stored = localStorage.getItem("user");
  if (stored) {
    try {
      user = JSON.parse(stored);
      updateNav();
    } catch {
      localStorage.removeItem("user");
      user = null;
    }
  }

  // Auto-route only on index.html
  const path = window.location.pathname;
  const isIndex = path === '/' || path.endsWith('index.html') || path === '';
  
  updateNav(); // Ensure nav is updated on all pages

  if (isIndex) {
    if (user) {
      if (user.userType === 'Admin') show('admin');
      else show('dashboard');
    } else {
      show("home");
    }
  }
});

// =====================
// GLOBAL ALIASES — used by auth.js, properties.js and inline HTML
// =====================

function showMessage(text, type) {
  msg(text, type);
}

window.showMessage      = showMessage;
window.navigateTo       = navigateTo;
window.show             = show;
window.toggleChat       = toggleChat;
window.sendChat         = sendChat;
window.switchToRegister = switchToRegister;
window.switchToLogin    = switchToLogin;
window.toggleAuthMode   = toggleAuthMode;
window.logout           = logout;
window.handleLogout     = logout;
window.togglePropertyForm = togglePropertyForm;
window.showAdminTab       = showAdminTab;
window.toggleAdminSecret  = toggleAdminSecret;
window.startVoiceRecognition = startVoiceRecognition;
window.renderProfile = renderProfile;
window.clearChat = clearChat;
window.contactSeller = window.contactProperty; // Alias for backward compatibility if any
