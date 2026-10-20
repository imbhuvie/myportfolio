
// ---------- theme toggle (in-memory, no storage) ----------
const root = document.documentElement;
const knob = document.getElementById('themeKnob');
document.getElementById('themeToggle').addEventListener('click', () => {
  const isDark = root.getAttribute('data-theme') === 'dark';
  root.setAttribute('data-theme', isDark ? 'light' : 'dark');
  knob.textContent = isDark ? '☀️' : '🌙';
});

// ---------- gutter line numbers ----------
const gutter = document.getElementById('gutter');
const codeLineCount = document.querySelectorAll('.code-pane > .code-line').length;
for(let i=1;i<=codeLineCount;i++){
  const d = document.createElement('div');
  d.textContent = i;
  gutter.appendChild(d);
}

// ---------- typing effect ----------
const phrases = [
  "console.log('turning business problems into software');",
  "// automation that pays for itself",
  "> dotnet run --project Your.NextProject"
];
let pIndex = 0, cIndex = 0, deleting = false;
const typedEl = document.getElementById('typedText');
function typeLoop(){
  const current = phrases[pIndex];
  if(!deleting){
    typedEl.textContent = current.slice(0, cIndex+1);
    cIndex++;
    if(cIndex === current.length){ deleting = true; setTimeout(typeLoop, 1600); return; }
  } else {
    typedEl.textContent = current.slice(0, cIndex-1);
    cIndex--;
    if(cIndex === 0){ deleting = false; pIndex = (pIndex+1) % phrases.length; }
  }
  setTimeout(typeLoop, deleting ? 28 : 45);
}
typeLoop();

// ---------- terminal lines fade in sequentially ----------
document.querySelectorAll('.terminal-body .line').forEach((line, i)=>{
  setTimeout(()=>{ line.style.transition='opacity .4s ease'; line.style.opacity=1; }, 500 + i*450);
});

// ---------- scroll reveal ----------
const revealEls = document.querySelectorAll('.reveal');
const io = new IntersectionObserver((entries)=>{
  entries.forEach(e=>{
    if(e.isIntersecting){ e.target.classList.add('in'); io.unobserve(e.target); }
  });
}, {threshold:0.15});
revealEls.forEach(el=>io.observe(el));

// ---------- tab nav: click + scroll spy ----------
const tabs = document.querySelectorAll('.tab');
const targets = Array.from(tabs).map(t => document.getElementById(t.dataset.target));
tabs.forEach(tab=>{
  tab.addEventListener('click', ()=>{
    document.getElementById(tab.dataset.target).scrollIntoView({behavior:'smooth'});
  });
});
const spy = new IntersectionObserver((entries)=>{
  entries.forEach(entry=>{
    if(entry.isIntersecting){
      const id = entry.target.id;
      tabs.forEach(t => t.classList.toggle('active', t.dataset.target === id));
    }
  });
}, {rootMargin:'-40% 0px -55% 0px'});
targets.forEach(t => t && spy.observe(t));

// ---------- copy to clipboard ----------
function copyText(text, btn){
  navigator.clipboard.writeText(text).then(()=>{
    const original = btn.textContent;
    btn.textContent = 'Copied ✓';
    btn.classList.add('copied');
    setTimeout(()=>{ btn.textContent = original; btn.classList.remove('copied'); }, 1600);
  });
}

// ---------- contact form (AJAX) ----------
const contactForm = document.getElementById('contactForm');
if (contactForm) {
  contactForm.addEventListener('submit', async (e) => {
    e.preventDefault();
    const status = document.getElementById('formStatus');
    const btn = contactForm.querySelector('button[type="submit"]');

    btn.disabled = true;
    btn.textContent = 'Sending...';
    status.className = 'form-status';
    status.textContent = '';

    try {
      const res = await fetch('/Contact/Send', { method: 'POST', body: new FormData(contactForm) });
      const json = await res.json();
      if (json.success) {
        status.textContent = '✓ ' + json.message;
        status.classList.add('success');
        contactForm.reset();
      } else {
        status.textContent = json.message || 'Something went wrong. Please try again or email me directly.';
        status.classList.add('error');
      }
    } catch (err) {
      status.textContent = 'Network error. Please email bhupendraverma7388@gmail.com directly.';
      status.classList.add('error');
    } finally {
      btn.disabled = false;
      btn.textContent = 'Send Project Brief →';
    }
  });
}
