// Auth.js — handles login & register form submissions
console.log('Auth.js loaded');

document.addEventListener('DOMContentLoaded', () => {
  const loginForm = document.getElementById('loginForm');
  if (loginForm) loginForm.addEventListener('submit', handleLogin);

  const registerForm = document.getElementById('registerForm');
  if (registerForm) registerForm.addEventListener('submit', handleRegister);
});

async function handleLogin(e) {
  e.preventDefault();

  const username = document.getElementById('loginUsername').value;
  const password = document.getElementById('loginPassword').value;

  try {
    const response = await fetch(`${API_URL}/auth/login`, {
      method: 'POST',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify({ username, password })
    });
    
    const data = await response.json();
    
    if (response.ok) {
        localStorage.setItem('token', data.token);
        localStorage.setItem('user', JSON.stringify(data));
        
        user = data; // Hydrate global immediately
        updateNav();
        
        if (user.userType === 'Admin') show('admin');
        else show('dashboard');
        
        showMessage('✅ Login successful!', 'success');
    } else {
      showMessage('❌ ' + (data.message || 'Login failed'), 'error');
    }
  } catch (error) {
    showMessage('❌ Connection error. Please try again later.', 'error');
    console.error('Login error:', error);
  }
}

async function handleRegister(e) {
  e.preventDefault();

  const username  = document.getElementById('regUsername').value;
  const email     = document.getElementById('regEmail').value;
  const password  = document.getElementById('regPassword').value;
  const firstName = document.getElementById('regFirstName').value;
  const lastName  = document.getElementById('regLastName').value;
  const phone     = document.getElementById('regPhone').value;
  const address   = document.getElementById('regAddress').value;
  const userType  = document.getElementById('regUserType').value;
  const adminSec  = document.getElementById('regAdminSecret').value;

  if (!username || !email || !password || !phone) {
    showMessage('❌ Please fill all required fields.', 'error');
    return;
  }

  if (password.length < 6) {
    showMessage('❌ Password must be at least 6 characters long.', 'error');
    return;
  }

  if (userType === 'Admin' && adminSec !== 'ADMIN777') {
    showMessage('❌ Invalid Admin Secret Code', 'error');
    return;
  }

  try {
    const response = await fetch(`${API_URL}/auth/register`, {
      method: 'POST',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify({ 
        username, email, password, firstName, lastName, phone, address, userType
      })
    });

    const data = await response.json();

    if (response.ok || data.success) {
      document.getElementById('registerForm').reset();
      
      showMessage('✅ Registration successful! Please login.', 'success');
      setTimeout(() => {
        if (typeof toggleAuthMode === 'function') {
           toggleAuthMode('login');
        } else {
           document.getElementById('registerBox').style.display = 'none';
           document.getElementById('loginBox').style.display = 'block';
        }
      }, 1500);
    } else {
      // Handle ModelState errors or custom error messages
      let errorMsg = 'Registration failed.';
      if (data.errors) {
         const firstErrorKey = Object.keys(data.errors)[0];
         errorMsg = data.errors[firstErrorKey][0];
      } else if (data.message) {
         errorMsg = data.message;
      }
      showMessage('❌ ' + errorMsg, 'error');
    }
  } catch (error) {
    console.error('Register error:', error);
    showMessage('❌ Registration error. Please try again.', 'error');
  }
}