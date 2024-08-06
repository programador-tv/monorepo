


function JoinTimeForRequestedHelp(time, perfil, group,index) {
        index++

        let prepared = `
        <div
        class="${group} item-preview-ts"
        onclick="SetItToTryRequestedHelp('${time.timeSelectionId}', '#freeTimes-${perfil.id}')"
        style="display:${index <= 2 ?'flex' : 'none'};" >

            <div style="display:flex;min-width: 100%;">
                    <span style="padding:5px;margin-top: 5px;" >
                    ${renderAvatar(perfil, true)}
                    </span>
                <span style="display: flex;flex-direction: column; color: #575757;width: 100%;">
                        <div class="d-flex justify-content-center align-items-center" style="background-color: #FFFFFF;border: 1px solid #CECECE;font-size: 11px;border-radius: 8px;width: 63px;height: 17px;padding: 2px 3px;align-self: end;">
                            ${ experience[perfil.experiencia]} xp
                        </div>
                        <h2 style="font-size: 16px;font-weight: 700;letter-spacing: 0.015em;
                        text-align: left;margin-bottom: 2px;">${perfil.nome}</h2>
                        <h3 style="font-size: 12px;font-weight: 500;letter-spacing: 0em;
                        text-align: left;margin-bottom: 2px;">${perfil.bio ?? ""}</h3>

                </span>
            </div>`

        prepared += `<div style="width:100%; height:2px; background:rgb(215, 215, 215)"></div>`
        prepared += `
        <div style="text-align: left;width: 67%;height: 50%;padding: 8px;">
            <h2 style="color: #734B95;letter-spacing: 0.015em;
            text-align: left;font-size: 18px;font-weight: 400;
            height: 75px; word-wrap: break-word;">${time.title.length > 50 ? time.title.substring(0, 50) + "..." : time.title}</h2>
            <div style="display: flex; flex-wrap: wrap;    width: 90%;">`

            prepared += `<small class="tag-capacitacao tag-duvida mt-0">
													${mapTipo[time.variation]}
												</small>`;
            prepared += `<small class="tag-capacitacao mt-0">
													${time.tags[0].titulo.length > 20
														? time.tags[0].titulo.substring(0, 20) + "..."
														: time.tags[0].titulo}
												</small>`;

        prepared += `</div>
        </div>`


        prepared += `<div style="width: 33%;height: 50%; padding:8px">`
            prepared += `<p style="color: #525252;
            margin-bottom: 0;
            line-height: 18px;letter-spacing: 0em;text-align: right;">
            Dia ${new Date(time.startTime).getDate()} de ${mesesAbrev[new Date(time.startTime).getMonth()]}
            </p>

            <p style="color: #525252 ; 500; font-size: 14px;
        line-height: 15px;letter-spacing: 0em;text-align: right;">

            ${new Date(time.startTime).toLocaleTimeString('pt-BR', {hour: '2-digit', minute: '2-digit'})} - ${new Date(time.endTime).toLocaleTimeString('pt-BR', {hour: '2-digit', minute: '2-digit'})}

            </p>`

        prepared += `</div>`


        prepared += `<div style="width: 99%;
        text-align: right;" >`
            prepared += `<span>
                            <b style="font-size: 25px;font-weight: 700;line-height: 23px;">${diferencaEmMinutos(new Date(time.endTime), new Date(time.startTime))} min </b> <small>duração</small>
                        </span>`
        prepared += `</div>`



        prepared += `</div>`;


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
        const qtdTempo = qtdEmMinutosCriada(item.createdAt);
        let botaoDeletar = '<div id="container-btn-deleteResponse"></div>';
        prepared +=
            `<div class="container-helpResponse" id="helpResponse-${item.id}">
                <div class="content-helpResponse">
                    <div id="container-description-helpResponse">
                        <p id="description-helpResponse">${item.conteudo}</p>
                    </div>

                    <div id="container-time">
                        <p>${qtdTempo.valor}${qtdTempo.tag}</p>
                    </div>
                </div>
            `
        if (userLoggedId === perfilId || userLoggedId === item.perfilId) {
            botaoDeletar = `
                <div id="container-btn-deleteResponse">
                    <span onclick=deleteHelpResponse('${item.id}') id="btn-deleteResponse">🗑️</span>
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
        showsCommentMessageSent();
        setTimeout(() => {
            $("#container-msg-helpResponse").html("");
        }, 3000)
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
    })
}


function showsCommentMessageSent() {
    let prepared = "";

    prepared += `
        <div class="content-msg-helpResponse">
            <div class="alert alert-success" role="alert">
                Você contribuiu com o pedido de ajuda através de um comentário.
            </div>
        </div>
    `;

    $("#container-msg-helpResponse").html(prepared);
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
