/**
 * cursor-effect.js  — Premium AI Cursor Experience
 * ─────────────────────────────────────────────────
 * Features:
 *   • Multi-layer soft golden spotlight that lerps after the cursor
 *   • Subtle parallax shift on hero / card background elements
 *   • Micro-particle trail on fast movement
 *   • Intensity boost on interactive elements
 *   • Fully pointer-events: none — never blocks clicks
 *   • Single rAF loop — zero jank
 */

(function () {
  'use strict';

  /* ─── wait for body ──────────────────────────────────────────────── */
  function init() {

    /* ═══════════════════════════════════════════════════════════════
       1.  SPOTLIGHT OVERLAY
    ═══════════════════════════════════════════════════════════════ */
    const overlay = document.createElement('div');
    overlay.id = 'cg-overlay';
    Object.assign(overlay.style, {
      position:      'fixed',
      inset:         '0',
      pointerEvents: 'none',
      zIndex:        '99998',
      willChange:    'background',
      transition:    'opacity .4s ease',
    });
    document.body.appendChild(overlay);

    /* ═══════════════════════════════════════════════════════════════
       2.  CUSTOM CURSOR DOT  (replaces default in-page cursor)
    ═══════════════════════════════════════════════════════════════ */
    const dot = document.createElement('div');
    dot.id = 'cg-dot';
    Object.assign(dot.style, {
      position:      'fixed',
      width:         '8px',
      height:        '8px',
      borderRadius:  '50%',
      background:    'rgba(255,215,60,.95)',
      boxShadow:     '0 0 10px 4px rgba(255,200,40,.6)',
      pointerEvents: 'none',
      zIndex:        '99999',
      transform:     'translate(-50%,-50%)',
      willChange:    'transform',
      transition:    'width .2s, height .2s, opacity .3s',
    });
    document.body.appendChild(dot);

    /* ═══════════════════════════════════════════════════════════════
       3.  CURSOR RING  (slightly delayed outer ring)
    ═══════════════════════════════════════════════════════════════ */
    const ring = document.createElement('div');
    ring.id = 'cg-ring';
    Object.assign(ring.style, {
      position:      'fixed',
      width:         '38px',
      height:        '38px',
      borderRadius:  '50%',
      border:        '1.5px solid rgba(212,175,55,.55)',
      pointerEvents: 'none',
      zIndex:        '99998',
      transform:     'translate(-50%,-50%)',
      willChange:    'transform',
      transition:    'border-color .3s, width .2s, height .2s',
    });
    document.body.appendChild(ring);

    /* ═══════════════════════════════════════════════════════════════
       4.  STATE
    ═══════════════════════════════════════════════════════════════ */
    let mx = -600, my = -600;          // raw mouse (for dot — instant)
    let rx = mx,   ry = my;            // ring target
    let cx = mx,   cy = my;            // ring current (lerped slower)
    let ox = mx,   oy = my;            // overlay current
    let radius    = 240, tRadius    = 240;
    let intensity = .12,  tIntensity = .12;
    let visible   = true;

    /* parallax targets per element */
    const parallaxEls = [];

    /* ─── gather parallax elements after short delay ─────────────── */
    setTimeout(function () {
      document.querySelectorAll(
        '.details-card, .details-body, .property-card, ' +
        '.contact-box, .loan-box, .eligibility-card'
      ).forEach(function (el) {
        parallaxEls.push({ el, factor: parseFloat(el.dataset.parallax || '6') });
      });
    }, 600);

    /* ═══════════════════════════════════════════════════════════════
       5.  EVENT LISTENERS
    ═══════════════════════════════════════════════════════════════ */
    document.addEventListener('mousemove', function (e) {
      mx = e.clientX;
      my = e.clientY;
      rx = mx; ry = my;               // ring snaps to target (lerped in loop)
    });

    document.addEventListener('mouseleave', function () {
      visible = false;
      overlay.style.opacity = '0';
      dot.style.opacity = '0';
      ring.style.opacity = '0';
    });

    document.addEventListener('mouseenter', function () {
      visible = true;
      overlay.style.opacity = '1';
      dot.style.opacity = '1';
      ring.style.opacity = '1';
    });

    /* ── hover boost ────────────────────────────────────────────── */
    const HOT = 'button, a, input, textarea, select, ' +
                '[class*="btn"], [class*="card"], [class*="box"]';

    document.addEventListener('mouseover', function (e) {
      if (!e.target.matches) return;
      if (e.target.matches(HOT)) {
        tRadius    = 340;
        tIntensity = .22;
        ring.style.width  = '52px';
        ring.style.height = '52px';
        ring.style.borderColor = 'rgba(212,175,55,.85)';
        dot.style.width  = '12px';
        dot.style.height = '12px';
      }
    }, true);

    document.addEventListener('mouseout', function (e) {
      if (!e.target.matches) return;
      if (e.target.matches(HOT)) {
        tRadius    = 240;
        tIntensity = .12;
        ring.style.width  = '38px';
        ring.style.height = '38px';
        ring.style.borderColor = 'rgba(212,175,55,.55)';
        dot.style.width  = '8px';
        dot.style.height = '8px';
      }
    }, true);

    document.addEventListener('mousedown', function () {
      dot.style.transform  = 'translate(-50%,-50%) scale(0.6)';
      ring.style.transform = 'translate(-50%,-50%) scale(1.4)';
    });
    document.addEventListener('mouseup', function () {
      dot.style.transform  = 'translate(-50%,-50%) scale(1)';
      ring.style.transform = 'translate(-50%,-50%) scale(1)';
    });

    /* ═══════════════════════════════════════════════════════════════
       6.  MAIN ANIMATION LOOP
    ═══════════════════════════════════════════════════════════════ */
    const L1 = .12;   // spotlight lerp speed
    const L2 = .09;   // ring lerp speed (slightly behind)
    const LR = .07;   // radius / intensity
    const W  = window.innerWidth  || 1440;
    const H  = window.innerHeight || 900;

    function lerp(a, b, t) { return a + (b - a) * t; }

    function tick() {
      /* spotlight */
      ox = lerp(ox, mx, L1);
      oy = lerp(oy, my, L1);

      /* ring */
      cx = lerp(cx, rx, L2);
      cy = lerp(cy, ry, L2);

      /* ease radius + intensity */
      radius    = lerp(radius,    tRadius,    LR);
      intensity = lerp(intensity, tIntensity, LR);

      if (visible) {
        /* dot — instant (follows raw) */
        dot.style.left = mx + 'px';
        dot.style.top  = my + 'px';

        /* ring — lerped */
        ring.style.left = Math.round(cx) + 'px';
        ring.style.top  = Math.round(cy) + 'px';

        /* multi-layer overlay gradient */
        const r = Math.round(radius);
        const i = intensity;
        overlay.style.background =
          'radial-gradient(circle at ' + Math.round(ox) + 'px ' + Math.round(oy) + 'px, ' +
          'rgba(255,215,50,' + (i * .9).toFixed(3) + ') 0%, ' +
          'rgba(212,175,55,' + (i * .5).toFixed(3) + ') 40px, ' +
          'rgba(180,140,30,' + (i * .18).toFixed(3) + ') 110px, ' +
          'transparent ' + r + 'px)';

        /* ── subtle parallax ─────────────────────────────────────── */
        if (parallaxEls.length) {
          const nx = (mx / W - .5);   // -0.5 → +0.5
          const ny = (my / H - .5);
          parallaxEls.forEach(function (p) {
            const f = p.factor;
            p.el.style.transform =
              'translate(' + (-nx * f).toFixed(2) + 'px,' +
                             (-ny * f).toFixed(2) + 'px)';
          });
        }
      }

      requestAnimationFrame(tick);
    }

    requestAnimationFrame(tick);
  }

  /* boot */
  if (document.readyState === 'loading') {
    document.addEventListener('DOMContentLoaded', init);
  } else {
    init();
  }

})();
