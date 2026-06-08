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

// Carousel functionality
class Carousel {
  constructor(element) {
    this.carousel = element;
    this.track = element.querySelector('.carousel-track');
    this.slides = element.querySelectorAll('.carousel-slide');
    this.dots = element.querySelectorAll('.carousel-dot');
    this.prevBtn = null;
    this.nextBtn = null;
    
    this.currentSlide = 0;
    this.totalSlides = this.slides.length;
    this.autoPlayInterval = null;
    
    if (this.totalSlides <= 1) return;
    
    this.init();
  }
  
  init() {
    this.dots.forEach((dot, index) => {
      dot.addEventListener('click', () => this.goToSlide(index));
    });
    
    this.startAutoPlay();
    this.carousel.addEventListener('mouseenter', () => this.stopAutoPlay());
    this.carousel.addEventListener('mouseleave', () => this.startAutoPlay());
  }
  
  updateSlide() {
    const offset = -this.currentSlide * 100;
    this.track.style.transform = `translateX(${offset}%)`;
    
    this.dots.forEach((dot, index) => {
      dot.classList.toggle('active', index === this.currentSlide);
    });
  }
  
  next() {
    this.currentSlide = (this.currentSlide + 1) % this.totalSlides;
    this.updateSlide();
  }
  
  goToSlide(index) {
    this.currentSlide = index;
    this.updateSlide();
  }
  
  startAutoPlay() {
    this.autoPlayInterval = setInterval(() => this.next(), 4500);
  }
  
  stopAutoPlay() {
    clearInterval(this.autoPlayInterval);
  }
  
  restartAutoPlay() {
    this.stopAutoPlay();
    this.startAutoPlay();
  }
}

// Initialize carousels
document.querySelectorAll('.carousel').forEach(element => {
  new Carousel(element);
});
