function drawPieChart3d(canvasId, tv, ta, lblTV, lblTA) {
    var canvas = document.getElementById(canvasId);
    if (!canvas) return;
    var ctx = canvas.getContext('2d');
    var W = canvas.width, H = canvas.height;
    var cx = W / 2, cy = H * 0.38;
    var rx = W * 0.40, ry = rx * 0.32, depth = 45;
    var total = tv + ta;
    if (total === 0) return;

    var slices = [
        { value: tv, color: '#1F3864', dark: '#0d1f3f', label: lblTV },
        { value: ta, color: '#A8A8A8', dark: '#6a6a6a', label: lblTA }
    ];

    var angle = -Math.PI / 2;
    slices.forEach(function (s) {
        s.sa = angle;
        s.ea = angle + (s.value / total) * 2 * Math.PI;
        s.mid = (s.sa + s.ea) / 2;
        angle = s.ea;
    });

    function drawSide(s) {
        var va = Math.max(s.sa, 0);
        var vb = Math.min(s.ea, Math.PI);
        if (va >= vb) return;
        ctx.beginPath();
        ctx.moveTo(cx + rx * Math.cos(va), cy + ry * Math.sin(va));
        ctx.ellipse(cx, cy, rx, ry, 0, va, vb, false);
        ctx.lineTo(cx + rx * Math.cos(vb), cy + depth + ry * Math.sin(vb));
        ctx.ellipse(cx, cy + depth, rx, ry, 0, vb, va, true);
        ctx.closePath();
        ctx.fillStyle = s.dark;
        ctx.fill();
        ctx.strokeStyle = 'rgba(0,0,0,0.1)';
        ctx.lineWidth = 0.5;
        ctx.stroke();
    }

    function drawTop(s) {
        ctx.beginPath();
        ctx.moveTo(cx, cy);
        ctx.ellipse(cx, cy, rx, ry, 0, s.sa, s.ea, false);
        ctx.closePath();
        ctx.fillStyle = s.color;
        ctx.fill();
        ctx.strokeStyle = 'rgba(0,0,0,0.2)';
        ctx.lineWidth = 1;
        ctx.stroke();
    }

    slices.slice()
        .sort(function (a, b) {
            var ma = (Math.max(a.sa, 0) + Math.min(a.ea, Math.PI)) / 2;
            var mb = (Math.max(b.sa, 0) + Math.min(b.ea, Math.PI)) / 2;
            return Math.abs(mb - Math.PI / 2) - Math.abs(ma - Math.PI / 2);
        })
        .forEach(drawSide);
    slices.forEach(drawTop);

    slices.forEach(function (s) {
        var lx = cx + rx * 0.65 * Math.cos(s.mid);
        var ly = cy + ry * 0.65 * Math.sin(s.mid);
        ctx.fillStyle = '#ffffff';
        ctx.font = 'bold 11px Arial';
        ctx.textAlign = 'center';
        ctx.textBaseline = 'middle';
        ctx.fillText(s.label, lx, ly);
    });
}
