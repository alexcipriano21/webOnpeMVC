document.addEventListener('DOMContentLoaded', function () {
    const rawData = document.getElementById('ubigeoData')?.textContent;
    let dataNacional = [];
    let dataExtranjero = [];
    if (rawData) {
        const parsed = JSON.parse(rawData);
        dataNacional = parsed.nacional || [];
        dataExtranjero = parsed.extranjero || [];
    }

    const selAmbito = document.getElementById('selAmbito');
    const lblNivel1 = document.getElementById('lblNivel1');
    const lblNivel2 = document.getElementById('lblNivel2');
    const lblNivel3 = document.getElementById('lblNivel3');
    const lblNivel4 = document.getElementById('lblNivel4');

    if (selAmbito) {
        selAmbito.addEventListener('change', function () {
            const val = this.value;
            resetSelect('selProvincia');
            resetSelect('selDistrito');
            resetSelect('selLocal');
            ocultarMesas();

            if (val === 'Nacional') {
                lblNivel1.textContent = 'Departamento:';
                lblNivel2.textContent = 'Provincia:';
                lblNivel3.textContent = 'Distrito:';
                lblNivel4.textContent = 'Locales:';
                llenarSelectLocal('selDepartamento', dataNacional, 'id', 'nombre');
            } else {
                lblNivel1.textContent = 'Continente:';
                lblNivel2.textContent = 'País:';
                lblNivel3.textContent = 'Ciudad:';
                lblNivel4.textContent = 'Locales:';
                llenarSelectLocal('selDepartamento', dataExtranjero, 'id', 'nombre');
            }
        });
        selAmbito.dispatchEvent(new Event('change'));
    }

    const selDepartamento = document.getElementById('selDepartamento');
    if (selDepartamento) {
        selDepartamento.addEventListener('change', function () {
            const id = this.value;
            resetSelect('selProvincia');
            resetSelect('selDistrito');
            resetSelect('selLocal');
            ocultarMesas();
            if (!id) return;

            fetch('/Onpe/GetProvincias?idDepartamento=' + id)
                .then(r => r.json())
                .then(data => {
                    llenarSelect('selProvincia', data, 'idProvincia', 'detalle');
                });
        });
    }

    const selProvincia = document.getElementById('selProvincia');
    if (selProvincia) {
        selProvincia.addEventListener('change', function () {
            const id = this.value;
            resetSelect('selDistrito');
            resetSelect('selLocal');
            ocultarMesas();
            if (!id) return;

            fetch('/Onpe/GetDistritos?idProvincia=' + id)
                .then(r => r.json())
                .then(data => {
                    llenarSelect('selDistrito', data, 'idDistrito', 'detalle');
                });
        });
    }

    const selDistrito = document.getElementById('selDistrito');
    if (selDistrito) {
        selDistrito.addEventListener('change', function () {
            const id = this.value;
            resetSelect('selLocal');
            ocultarMesas();
            if (!id) return;

            fetch('/Onpe/GetLocalesVotacion?idDistrito=' + id)
                .then(r => r.json())
                .then(data => {
                    llenarSelect('selLocal', data, 'idLocalVotacion', 'razonSocial');
                });
        });
    }

    const selLocal = document.getElementById('selLocal');
    if (selLocal) {
        selLocal.addEventListener('change', function () {
            const id = this.value;
            ocultarMesas();
            if (!id) return;

            fetch('/Onpe/GetGruposVotacion?idLocalVotacion=' + id)
                .then(r => r.json())
                .then(grupos => {
                    const tbody = document.getElementById('tbodyMesas');
                    if (tbody) {
                        tbody.innerHTML = '';
                        let tr = document.createElement('tr');
                        grupos.forEach((g, i) => {
                            if (i > 0 && i % 10 === 0) {
                                tbody.appendChild(tr);
                                tr = document.createElement('tr');
                            }
                            const td = document.createElement('td');
                            td.style.cursor = 'pointer';
                            td.style.backgroundColor = '#C1C1C1';
                            td.innerHTML = '<a href="#">' + g + '</a>';
                            td.addEventListener('click', function (e) {
                                e.preventDefault();
                                verDetalleMesa(g);
                            });
                            tr.appendChild(td);
                        });
                        tbody.appendChild(tr);
                        document.getElementById('divMesas').style.display = 'block';
                    }
                });
        });
    }
});

function verDetalleMesa(idGrupo) {
    fetch('/Onpe/GetGrupoVotacion?id=' + idGrupo)
        .then(r => r.json())
        .then(d => {
            if (d.error) { alert(d.error); return; }
            document.getElementById('detMesaId').textContent = d.idGrupoVotacion;
            document.getElementById('detNCopia').textContent = d.nCopia;
            document.getElementById('detDepartamento').textContent = d.departamento;
            document.getElementById('detProvincia').textContent = d.provincia;
            document.getElementById('detDistrito').textContent = d.distrito;
            document.getElementById('detRazonSocial').textContent = d.razonSocial;
            document.getElementById('detDireccion').textContent = d.direccion;
            document.getElementById('detElectores').textContent = d.electoresHabiles;
            document.getElementById('detVotantes').textContent = d.totalVotantes;
            document.getElementById('detEstado').textContent = d.estadoActaTexto;
            document.getElementById('detP1').textContent = d.p1;
            document.getElementById('detP2').textContent = d.p2;
            document.getElementById('detBlancos').textContent = d.votosBlancos;
            document.getElementById('detNulos').textContent = d.votosNulos;
            document.getElementById('detImpugnados').textContent = d.votosImpugnados;
            document.getElementById('detTotal').textContent = d.totalVotantes;

            const continentes = ['AFRICA', 'AMERICA', 'ASIA', 'EUROPA', 'OCEANIA'];
            const isExtranjero = continentes.includes((d.departamento || '').toUpperCase());

            document.getElementById('lblTDepartamento').textContent = isExtranjero ? 'Continente' : 'Departamento';
            document.getElementById('lblTProvincia').textContent = isExtranjero ? 'País' : 'Provincia';
            document.getElementById('lblTDistrito').textContent = isExtranjero ? 'Ciudad' : 'Distrito';

            const btnReg = document.getElementById('btnRegresar');
            if (btnReg) {
                if (isExtranjero) {
                    btnReg.classList.add('pull-right');
                } else {
                    btnReg.classList.remove('pull-right');
                }
            }

            document.getElementById('divDetalle').style.display = 'block';
            document.getElementById('divMesas').style.display = 'none';
        });
}

function ocultarDetalle() {
    const divDetalle = document.getElementById('divDetalle');
    const divMesas = document.getElementById('divMesas');
    if (divDetalle) divDetalle.style.display = 'none';
    if (divMesas) divMesas.style.display = 'block';
}

function ocultarMesas() {
    const divMesas = document.getElementById('divMesas');
    const divDetalle = document.getElementById('divDetalle');
    if (divMesas) divMesas.style.display = 'none';
    if (divDetalle) divDetalle.style.display = 'none';
}

function llenarSelectLocal(id, data, valKey, txtKey) {
    const sel = document.getElementById(id);
    if (!sel) return;
    sel.innerHTML = '<option value="">--SELECCIONE--</option>';
    data.forEach(item => {
        const opt = document.createElement('option');
        opt.value = item[valKey];
        opt.text = item[txtKey];
        sel.appendChild(opt);
    });
    sel.disabled = false;
}

function llenarSelect(id, data, valKey, txtKey) {
    const sel = document.getElementById(id);
    if (!sel) return;
    sel.innerHTML = '<option value="">--SELECCIONE--</option>';
    data.forEach(item => {
        const opt = document.createElement('option');
        opt.value = item[valKey];
        opt.text = item[txtKey];
        sel.appendChild(opt);
    });
    sel.disabled = false;
}

function resetSelect(id) {
    const sel = document.getElementById(id);
    if (!sel) return;
    sel.innerHTML = '<option value="">--SELECCIONE--</option>';
    sel.disabled = true;
}
