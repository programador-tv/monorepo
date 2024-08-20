function JoinTimeForRequestedHelp(time, perfil, group, index) {
	index++

	let prepared =
		`<div class="${group} item-preview-ts"
			onclick="SetItToTryRequestedHelp('${time.timeSelectionId}', '#freeTimes-${perfil.id}')"
			style="display:${index <= 2 ? 'flex' : 'none'};" >

			<header class="d-flex align-items-center w-100 py-3 px-4"
				style="background: linear-gradient(96.12deg, #AE87CF 0.79%, #995ECB 110%); height: 84px;">
				<a href='/Canal/Index?usr=${perfil.userName}'
					title="${perfil.userName}"
					target="_blank"
					style="margin-right: 10px;"
				>
					<img
						onerror="if (this.src != '/no-user.svg') this.src = '/no-user.svg';"
						src="/${perfil.foto}"
						class="rounded-circle" style="width: 52px; height: 52px;">
        </a>
				<span class="d-flex flex-column flex-fill"
					style="color: #FEFBFB">
					<h2 class="fs-6 fw-bold"
						style="letter-spacing: 0.015em; margin-bottom: 2px;">
						${perfil.nome.length > 30
						? perfil.nome.substring(0, 30) + "..."
						: perfil.nome}
					</h2>
					<h3 style="font-size: 12px; margin-bottom: 2px;">
						${perfil.bio.length > 35
						? perfil.bio.substring(0, 35) + "..."
						: perfil.bio}
					</h3>
				</span>
				<small class="tag-capacitacao tag-duvida align-self-start mt-0">
					${mapTipo[time.variation]}
				</small>
			</header>

			<div class="d-flex flex-column justify-content-between w-100"
				style="padding: 12px 10px; height: 100px;">
				<h2 style="color: #1C1C1C; letter-spacing: 0.015em; font-size: 18px; word-wrap: break-word;">
					${time.title.length > 90
					? time.title.substring(0, 90) + "..."
					: time.title}
				</h2>
				<div class="d-flex flex-row-reverse w-100">
					<small class="tag-capacitacao" style="margin-right: 14px">
						${time.tags[0].titulo.length > 20
						? time.tags[0].titulo.substring(0, 20) + "..."
						: time.tags[0].titulo}
					</small>
				</div>
			</div>

			<footer class="d-flex justify-content-between align-items-center w-100"
				style="padding: 12px 24px; color: #1C1C1C; background-color: #F2F2F2; font-size: 12px; font-weight: 500;">
				<p class="mb-0">
					<svg xmlns="http://www.w3.org/2000/svg" width="12" height="12" fill="currentColor" class="bi bi-calendar4" viewBox="0 0 16 16">
						<path d="M3.5 0a.5.5 0 0 1 .5.5V1h8V.5a.5.5 0 0 1 1 0V1h1a2 2 0 0 1 2 2v11a2 2 0 0 1-2 2H2a2 2 0 0 1-2-2V3a2 2 0 0 1 2-2h1V.5a.5.5 0 0 1 .5-.5M2 2a1 1 0 0 0-1 1v1h14V3a1 1 0 0 0-1-1zm13 3H1v9a1 1 0 0 0 1 1h12a1 1 0 0 0 1-1z"/>
					</svg>
					<span class="ms-1 me-2 fw-normal"
						style="color: #525252;">
						Datas
					</span>
					${new Date(time.startTime).getDate()} - ${mesesAbrev[new Date(time.startTime).getMonth()]}
				</p>
				<p class="mb-0">
					<svg xmlns="http://www.w3.org/2000/svg" width="12" height="12" fill="currentColor" class="bi bi-clock" viewBox="0 0 16 16">
						<path d="M8 3.5a.5.5 0 0 0-1 0V9a.5.5 0 0 0 .252.434l3.5 2a.5.5 0 0 0 .496-.868L8 8.71z"/>
						<path d="M8 16A8 8 0 1 0 8 0a8 8 0 0 0 0 16m7-8A7 7 0 1 1 1 8a7 7 0 0 1 14 0"/>
					</svg>
					<span class="ms-1 me-2 fw-normal"
						style="color: #525252;">
						Horário
					</span>
					${new Date(time.startTime).toLocaleTimeString('pt-BR', {hour: '2-digit', minute: '2-digit'})} - ${new Date(time.endTime).toLocaleTimeString('pt-BR', {hour: '2-digit', minute: '2-digit'})}
				</p>
			</footer>
		</div>`;

    return prepared;
}

function SetItToTryRequestedHelp(freetimeId) {
    $(".modal").modal("hide")
    let perfil;
    let time;
    for (let element of result) {
        for (let item of element.timeSelections) {
            if (item.timeSelectionId == freetimeId) {
                time = item
                perfil = element.perfils
                break;
            }
        }
    }
    document.querySelector("#perfilJoinTime").value = perfil.id
    document.querySelector("#timeSelectionJoinTime").value = freetimeId
    setTimeout(() => {
        renderRequestedHelpSelected(perfil, time)
        renderRequestedHelpResponse(time.helpResponses, time.perfilId)
        $("#eventModal").modal("hide")
        $("#matcHelphModal").modal("show")
    }, 300)
}

function renderRequestedHelpResponse(listHelpResponse, perfilId) {
    const userLoggedId = document.getElementById("requestProfileId").value;
    if (userLoggedId !== perfilId) {
        let prepareFormHelpResponse = `
            <p>Comentar</p>
            <div class="response-text-area">
                <textarea class="form-control" id="contentHelpResponse"></textarea>
            </div>

            <div class="btn-add-helpResponse">
                <button id="btn-sendComment" onclick="sendComment()" type="button" class="button button-capacitacao">Enviar comentário</button>
            </div>
        `
        $("#help-response").html(prepareFormHelpResponse)
    }

    let prepared = "";
    for (let item of listHelpResponse) {
        const qtdTempo = qtdEmMinutosCriada(item.helpResponse.createdAt);
        let botaoDeletar = '<div id="container-btn-deleteResponse"></div>';
        let profile = { userName: item.profileUserName, nome: item.profileNome, foto: item.profileFoto };
        prepared +=
            `<div class="container-helpResponse" id="helpResponse-${item.helpResponse.id}">
                <div class="content-helpResponse">
                    <div class="container-profile-helpResponse">
                        <div class="img-profile-helpResponse">
                            <span>
                                ${renderAvatar(profile, true)}
                            </span>
                        </div>
                        <div class="nome-profile-helpResponse">
                            <span>${profile.nome}</span>
                        </div>
                    </div>

                    <div id="container-description-helpResponse">
                        <p id="description-helpResponse">${item.helpResponse.conteudo}</p>
                    </div>

                    <div id="container-time">
                        <p>${qtdTempo.valor}${qtdTempo.tag}</p>
                    </div>
                </div>
            `
        if (userLoggedId === perfilId || userLoggedId === item.helpResponse.perfilId) {
            botaoDeletar = `
                <div id="container-btn-deleteResponse">
                    <span onclick=deleteHelpResponse('${item.helpResponse.id}') id="btn-deleteResponse">
                        <svg xmlns="http://www.w3.org/2000/svg" width="16" height="16" fill="currentColor" class="bi bi-trash3" viewBox="0 0 16 16">
                          <path d="M6.5 1h3a.5.5 0 0 1 .5.5v1H6v-1a.5.5 0 0 1 .5-.5M11 2.5v-1A1.5 1.5 0 0 0 9.5 0h-3A1.5 1.5 0 0 0 5 1.5v1H1.5a.5.5 0 0 0 0 1h.538l.853 10.66A2 2 0 0 0 4.885 16h6.23a2 2 0 0 0 1.994-1.84l.853-10.66h.538a.5.5 0 0 0 0-1zm1.958 1-.846 10.58a1 1 0 0 1-.997.92h-6.23a1 1 0 0 1-.997-.92L3.042 3.5zm-7.487 1a.5.5 0 0 1 .528.47l.5 8.5a.5.5 0 0 1-.998.06L5 5.03a.5.5 0 0 1 .47-.53Zm5.058 0a.5.5 0 0 1 .47.53l-.5 8.5a.5.5 0 1 1-.998-.06l.5-8.5a.5.5 0 0 1 .528-.47M8 4.5a.5.5 0 0 1 .5.5v8.5a.5.5 0 0 1-1 0V5a.5.5 0 0 1 .5-.5"/>
                        </svg>
                    </span>
                </div>
            `
        }
        prepared += `
            ${botaoDeletar}
        </div>`
    }

    $("#space-helpResponses").html(prepared)
}

function sendComment() {
    const url = '?handler=HelpResponse';
    const formData = new FormData();
    if (!formData.has("__RequestVerificationToken")) {
        const token = document.querySelector("#forgery input").value
        formData.append("__RequestVerificationToken", token)
    }
    let timeSelectionId = document.getElementById("timeSelectionJoinTime").value;
    let contentHelpResponse = document.getElementById("contentHelpResponse").value;
    formData.append("timeSelectionId", timeSelectionId);
    formData.append("content", contentHelpResponse);

    const options = {
        method: "POST",
        body: formData,
    };

    fetch(url, options).then(function (response) {
        if (!response.ok) {
            throw new Error("Erro ao enviar comentário");
        }
        document.getElementById("contentHelpResponse").value = "";
        return response.json();
    }).then(function (data) {
        let freetimeId = document.querySelector("#timeSelectionJoinTime").value;
        let time;
        for (let element of result) {
            for (let item of element.timeSelections) {
                if (item.timeSelectionId == freetimeId) {
                    time = item
                    break;
                }
            }
        }
        time.helpResponses.unshift(data);
        renderRequestedHelpResponse(time.helpResponses, time.perfilId)
    });
}


function deleteHelpResponse(helpResponseId) {
    const url = '?handler=DeleteHelpResponse';
    const formData = new FormData();

    if (!formData.has("__RequestVerificationToken")) {
        const token = document.querySelector("#forgery input").value
        formData.append("__RequestVerificationToken", token)
    }
    formData.append("helpResponseId", helpResponseId);

    const options = {
        method: "POST",
        body: formData,
    }

    fetch(url, options).then(function (response) {
        if (!response.ok) {
            throw new Error("Erro ao deletar comentário.");
        }
        document.getElementById(`helpResponse-${helpResponseId}`).remove();
        let freetimeId = document.querySelector("#timeSelectionJoinTime").value;

        let time;
        for (let element of result) {
            for (let item of element.timeSelections) {
                if (item.timeSelectionId == freetimeId) {
                    time = item
                    break;
                }
            }
        }
        time.helpResponses = time.helpResponses.filter(e => e.helpResponse.id != helpResponseId);
    });
}


function qtdEmMinutosCriada(date) {
    const datetimeAtual = Date.now();
    const dataAtual = new Date(datetimeAtual).toISOString();
    const createdAt = new Date(date).toISOString();

    const diferencaEmMilissegundos = Date.parse(dataAtual) - Date.parse(createdAt)
    const diferencaSegundos = Math.floor(diferencaEmMilissegundos / 1000);
    const diferencaMinutos = Math.floor(diferencaSegundos / 60);

    if (diferencaMinutos < 60) {
        return diferencaMinutos == 0
            ? { tag: "Agora", valor: "" } : { tag: "min atrás", valor: diferencaMinutos }
    }
    else {
        const diferencaHoras = Math.floor(diferencaMinutos / 60);
        const diferencaDias = Math.floor(diferencaHoras / 24);

        return diferencaHoras > 48
            ? { tag: "D atrás", valor: diferencaDias } : { tag: "h atrás", valor: diferencaHoras }
    }

}

function renderRequestedHelpSelected(perfil, time) {

    let prepared = "";

    prepared += `<div class="detalhes-orientacao" >
        <div style="display: flex;justify-content: space-between;align-items: center;padding: 15px;">
            <p style="color: #9067B2;font-size: 25px;font-weight: 700;letter-spacing: 0.015em;margin: 0px;">
                Pedido de Ajuda
            </p>

            <button type="button" id="btnVoltar" data-bs-dismiss="modal" aria-label="Close" style="border: none;background-color: transparent;padding: 0px;">
                <img src="/Pictures/Icons/close-icon.svg" alt="icone voltar">
            </button>
        </div>
        <hr>`

    prepared += `<div style="display:flex;min-width: 100%;padding: 10px;">
        <span >
        ${renderAvatar(perfil, true)}
        </span>
        <span style="display: flex;flex-direction: column; color: #575757;width: fit-content;">
            <h2 style="font-size: 16px;font-weight: 700;letter-spacing: 0.015em;
            text-align: left;margin-bottom: 2px;">
                ${perfil.nome}
            </h2>
            <h3 style="font-size: 12px;font-weight: 500;letter-spacing: 0em;
            text-align: left;margin-bottom: 2px;">
                ${perfil.bio ?? ""}
            </h3>
            <div  class="d-flex justify-content-center align-items-center" style="background-color: #FFFFFF;border: 1px solid #CECECE;font-size: 11px;
            border-radius: 8px;width: 63px;height: 17px;padding: 2px 3px;">
                ${experience[perfil.experiencia]} xp
            </div>
        </span>
    </div>`

    prepared += `
        <p style="padding-top: 15px;font-size: 18px;font-weight: 700;margin: 0px; word-break: break-word;">${time.title}</p>
        <div class="d-flex justify-content-center align-items-center" style="flex-direction: column; padding: 20px;">
        `
        const dataInicio = new Date(time.startTime)
        const dataFim = new Date(time.endTime)

        const diaSemana = dataInicio.toLocaleDateString('pt-BR', { weekday: 'long' });

    prepared += `<h6 style="text-align: center;">
            <p style="color: #525252; font-size: 20px; font-weight: 500;margin: 0px;">

            ${diaSemana.replace(diaSemana.charAt(0),diaSemana.charAt(0).toLocaleUpperCase())}, ${dataInicio.getDate()} de
            ${mesesAbrev[dataInicio.getMonth()]}
            </p>
            <p style="color: #525252; font-size: 20px; font-weight: 500;">
            ${dataInicio.toLocaleTimeString('pt-BR', {hour12: false})
            .replace(":00", "")} -

            ${dataFim.toLocaleTimeString('pt-BR', {hour12: false})
            .replace(":00", "")}
            </p>
        </h6>
        <div style="gap:3px;display: flex; justify-content:center; flex-wrap:wrap;width: fit-content;">`



        if(time.tags.length > 0){
            for (let tag of time.tags) {
                prepared += `<small class="tag">${tag.titulo}</small>`
            }
        }

    prepared += `<div>
            </br>
                    <div style="word-break: break-all; text-align: justify">
                        <span>${time.description}</span>
                    </div>`
    if (time.imagePath && time.imagePath.length > 0) {
        prepared += `<div style="display: flex; justify-content: center">
                        <div class="helpBackstageImg-container">
                            <img class="helpBackstageImg" alt="Imagem de pedido de ajuda" src="${time.imagePath}" style="height: 500px; object-fit: contain;"/>
                            <a href="./${time.imagePath}" target="_blank" class="overlay"><strong style="font-size: 30px">Abrir imagem</strong></a>
                        </div>
                    </div>`
    }


    prepared += `</div>`
    prepared += `</div>
            </div>`


    $("#alunoMatch").html(prepared)
}
