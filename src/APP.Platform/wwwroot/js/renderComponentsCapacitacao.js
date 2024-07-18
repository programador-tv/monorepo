
const experience = {
    0: "< 1 ano",
    1: "1-3 anos",
    2: "3-5 anos",
    3: "5-7 anos",
    4: "> 8 anos",
}

function PrepareMentorCard(profileWithTimeSelections, alias) {


    const perfil = profileWithTimeSelections.perfil;
    const timeSelections = profileWithTimeSelections.ts;

    return JoinTimeForMentorias(timeSelections, perfil)

}
// function loadMentores(data) {
//     let prepared = ""
//     if (data.length == 0) {
//         $("#mentoriaBoard").html("Nenhuma mentoria encontrada no momento :/")
//         return;
//     }
//     for (const profileWithTimeSelections of data) {
//         result.push(profileWithTimeSelections)
//         prepared += PrepareMentorCard(profileWithTimeSelections, "Preview")
//     }
//     $("#mentoriaBoard").html(prepared)
// }
const mapTipo = {
    0 : "Sessão 1:1",
    1 : "Sessão 1:1",
    3 : "Evento",
    5 : "Dúvida",
}
function JoinTimeForMentorias(time, perfil, group, index) {
    index++
    let  prepared = `
        <div
        class="${group} item-preview-ts"
        onclick="SetItToTryFreeTime('${time.timeSelectionId}', '#freeTimes-${perfil.id}')"
        style="display:${index <= 2 ?'flex' : 'none'};" >

            <div style="display:flex;min-width: 100%;">
                    <span style="padding:5px;margin-top: 5px;" >
                    ${renderAvatar(perfil, true)}
                    </span>
                <span style="display: flex;flex-direction: column; color: #575757;width: 100%;">
                        <div class="d-flex justify-content-center align-items-center" style="background-color: #FFFFFF;border: 1px solid #CECECE;font-size: 11px;border-radius: 8px;width: 63px;height: 17px;padding: 2px 3px;align-self: end;">
                            ${experience[perfil.experiencia]} xp
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
            <h2 style="color: #734B95;letter-spacing: 0.015em;text-align: left;
            font-size: 18px;font-weight: 400; height: 75px; word-wrap: break-word;">${time.titulo.length > 50 ? time.titulo.substring(0, 50) + "..." : time.titulo}</h2>
            <div style="display: flex; flex-wrap: wrap;    width: 80%;">`

            prepared += `<small class="tag-capacitacao" style="border: solid 1px #1E53B7;background-color: #4C80E2; color: white;margin-top:0px">${mapTipo[time.variacao]}</small>`;
            prepared += `<small class="tag-capacitacao" style="margin-top:0px">${time.tags[0].titulo.length > 20 ? time.tags[0].titulo.substring(0, 20) + "..." : time.tags[0].titulo}</small>`;


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


function PrepareVacancy(time) {
    let vacancyEmojis = `<div style="margin-left: 7px;color: #373737; width: 170px;margin-bottom: 10px;">Vagas:<br>`;


    if (time.maxParticipantes > 1 && time.countInteressadosAceitos > 0 && time.countInteressadosAceitos != time.maxParticipantes) {
        vacancyEmojis += `/`;
    }

    vacancyEmojis += ` <img alt="icone vagas" src="/Pictures/Icons/vagas.svg">
                (${time.countInteressadosAceitos}/${time.maxParticipantes})
                <span style="font-size: 12px">${time.countInteressados} interessados</span>
            </div>
        `;

    return vacancyEmojis;
}

function SetItToTryFreeTime(freetimeId) {
    $(".modal").modal("hide")
    setTimeout(() => {
        renderFreeTimeSelected(freetimeId)
    }, 300)
}

function renderFreeTimeSelected(freetimeId) {

    let prepared = ""
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


    prepared += `<div class="detalhes-mentoria" >
        <div style="display: flex;justify-content: space-between;align-items: center;padding: 15px;">`

    prepared += `
            <p style="color: #9067B2;font-size: 25px;font-weight: 700;letter-spacing: 0.015em;margin: 0px;">
                ${time.variacao == 3 ? "Evento" : "Mentoria"}
            </p>`
    prepared += `
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
    /* if (perfil.linkedin) {
        prepared += `<a style="text-decoration:none"
                                        href="https://www.linkedin.com/in/${perfil.linkedin}/" target="_blank">
                                        <img style="width:30px;cursor:pointer" src="https://eduardoworrel.com/imgs/social/linkedin.png">
                                        </a>`
    }

    if (perfil.gitHub) {
        prepared += `<a style="text-decoration:none"
                                        href="https://github.com/${perfil.gitHub}/" target="_blank">
                                        <img style="width:30px;cursor:pointer" src="https://eduardoworrel.com/imgs/social/github (1).png">
                                        </a>`
    } */

    prepared += `
        <p style="padding-top: 15px;font-size: 18px;font-weight: 700;margin: 0px; word-break: break-word;">${time.titulo}</p>

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
    prepared += `</div>
            </div>`

    document.querySelector("#ownerProfileId").value = perfil.id
    document.querySelector("#JoinTime_TimeSelectionId").value = freetimeId


    $("#eventModal").modal("hide")

    $("#mentorMatch").html(prepared)
    $("#matchModal").modal("show")
}
