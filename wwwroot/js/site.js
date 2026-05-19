// Navbar scroll effect
const navbar = document.querySelector('.navbar');
if (navbar) {
  window.addEventListener('scroll', () => {
    navbar.classList.toggle('scrolled', window.scrollY > 20);
  });
}

// Stars render
document.querySelectorAll('.stars[data-rating]').forEach(el => {
  const r = parseFloat(el.dataset.rating);
  el.textContent = Array.from({length: 5}, (_, i) => i < Math.round(r) ? '★' : '☆').join('');
});

// Quantity controls
document.querySelectorAll('.qty-btn').forEach(btn => {
  btn.addEventListener('click', () => {
    const input = btn.closest('.qty-control').querySelector('.qty-input');
    const delta = btn.dataset.dir === 'up' ? 1 : -1;
    const newVal = Math.max(1, parseInt(input.value) + delta);
    input.value = newVal;
  });
});

// Auto-dismiss alerts
document.querySelectorAll('.alert').forEach(alert => {
  setTimeout(() => alert.style.opacity = '0', 3500);
});

// Chart bars animation
const bars = document.querySelectorAll('.chart-bar');
if (bars.length) {
  const vals = Array.from(bars).map(b => parseFloat(b.dataset.value));
  const max = Math.max(...vals);
  bars.forEach((bar, i) => {
    const pct = (vals[i] / max * 100);
    setTimeout(() => { bar.style.height = pct + '%'; }, i * 80);
  });
}
