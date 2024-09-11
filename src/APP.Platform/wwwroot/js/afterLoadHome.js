
const mesesAbrev = ["Janeiro", "Fevereiro", "Março", "Abril",
    "Maio", "Junho", "Julho", "Agosto", "Setembro", "Outubro", "Novembro", "Dezembro"];
let isUserLogged = false;
function diferencaEmMinutos(data1, data2) {
    const diferenca = Math.abs(data1.getTime() - data2.getTime());
    const diferencaEmMinutos = Math.round(diferenca / (1000 * 60));
    return diferencaEmMinutos;
}
let countLoaded = 0
const result = []

let countHelp = 0;
let countOneToOne = 0;
let countCursos = 0;

const countHelpElement = $("#count-help");
const countOneToOneElement = $("#count-one");
const countCursosElement = $("#count-cursos");
function updateOpenPanel(){
    if(countLoaded != 3){
        return
    }

    $(".tab-pane-events").removeClass("active");
    
    if (countHelp >= countOneToOne && countHelp >= countCursos) {
        countHelpElement.parent().addClass("active");
        $("#ajuda").addClass("active show");
    } else if (countOneToOne >= countHelp && countOneToOne >= countCursos) {
      countOneToOneElement.parent().addClass("active");
      $("#oneToOne").addClass("active show");
    } else {
      countCursosElement.parent().addClass("active");
      $("#cursos").addClass("active show");
    }
}

function afterLoad(isAuth) {
    const pedidosLoading = document.querySelector("#pedidosLoading");
    const videosLoading = document.querySelector("#videosLoading");
    const pedidosPanel = document.querySelector("#pedidosPanel");
    const savedvideos = document.querySelector("#savedVideos")
   
    fetch("?handler=AfterLoadLivePreview")
        .then(function (response) {
            if (!response.ok) {
                throw new Error("Erro ao enviar a solicitação GET.");
            }
            return response.json();
        })
        .then(async function (data) {

            if (data.preview) {
                videosLoading.remove();
                savedvideos.style.justifyContent = "start";
                savedvideos.innerHTML = data.preview + savedvideos.innerHTML;
            }
            fetch("?handler=AfterLoadLives")
            .then(function (response) {
                if (!response.ok) {
                    throw new Error("Erro ao enviar a solicitação POST.");
                }
                return response.json();
            })
            .then(function (data) {
                videosLoading.remove();
                savedvideos.style.justifyContent = "start";
                if (data.lives) {
                    savedvideos.innerHTML =  data.lives + savedvideos.innerHTML;
                }
            }).finally(function () {
                videosLoading.remove();
                savedvideos.style.justifyContent = "start";
            })

       


        })
        // loadingSpin.forEach(e => e.style.display = "none");




    // fetch("/Aprender?handler=RenderRightCardsEvent")
    // .then(function (response) {
    //     if (!response.ok) {
    //         throw new Error("Erro ao enviar a solicitação POST.");
    //     }
    //     return response.json();
    // })
    //  .then((data) => {
    //      let aside = document.querySelector("#theGreatAside")
    //      aside.innerHTML += data.modals
    //  })

    // fetch("?handler=AfterLoadMentores")
    //     .then(function (response) {
    //         if (!response.ok) {
    //             throw new Error("Erro ao enviar a solicita��o POST.");
    //         }
    //         return response.json();
    //     })
    //     .then(function (data) {
    //         isUserLogged = data.isLogged;
    //         loadMentores(data.mentores)
    //     }).catch(function (error) {
    //         console.log(error)
    //         $("#mentoriaBoard").html("Nenhuma mentoria encontrada no momento :/")
    //     })

    fetch("/ScheduleActions?handler=AfterLoadRequestedHelp")
        .then(function (response) {
            if (!response.ok) {
                throw new Error("Erro ao enviar a solicita??o POST.");
            }
            return response.json();
        })
        .then(function (data) {
            isUserLogged = data.isLogged;
            loadPedidosAjudaParaTag(data.pedidos)
        })
        .catch(function (error) {
            console.log(error)
        })

    fetch("?handler=AfterLoadSavedVideos")
        .then(function (response) {
            if (!response.ok) {
                throw new Error("Erro ao enviar a solicitação POST.");
            }
            return response.json();
        })
        .then(function (data) {

            videosLoading.remove();
            savedvideos.style.justifyContent = "start";
            if (data.saved) {
                savedvideos.innerHTML += data.saved;

            }
        })

    fetch("/ScheduleActions?handler=AfterLoadMentores"
    )
        .then(function (response) {
            if (!response.ok) {
                throw new Error("Erro ao enviar a solicita��o POST.");
            }
            return response.json();
        })
        .then(function (data) {
            pedidosLoading.remove();
            pedidosPanel.style.justifyContent = "space-between";
            isUserLogged = data.isLogged;
            loadMentoresParaTag(data.mentores)
        }).catch(function (error) {
            console.log(error)
            $("#OneToOneMatch").html("Falha ao encontrar mentor :/")
            $("#CursosMatch").html("Falha ao encontrar mentor :/")
        })

    // fetch("?handler=AfterLoadRequestedHelp")
    //     .then(function (response) {
    //         if (!response.ok) {
    //             throw new Error("Erro ao enviar a solicita??o POST.");
    //         }
    //         return response.json();
    //     })
    //     .then(function (data) {
    //         isUserLogged = data.isLogged;
    //         loadRequestedHelp(data.pedidos)
    //     })
    //     .catch(function (error) {
    //         console.log(error)
    //     })

    
}

function loadMentoresParaTag(data) {

    let prepared = ""
    if (data.length == 0) {
        prepared = "<div style='justify-content: center; display: flex; padding: 20px; color: grey;'>Sem mentorias no momento <div>"
        $("#OneToOneMatch").html(prepared)
    } else {
        let preparedOneToOne = ""


        let preparedCursos = ""

        let groupOneToOne = []
        let groupCursos = []
        for (const profileWithTimeSelections of data) {
            result.push(profileWithTimeSelections)
            groupOneToOne = [
                ...groupOneToOne,
                ...
                profileWithTimeSelections.timeSelections.filter(x => x.variacao == 1 || x.variacao == 0)
                    .map(e => {
                        return { ts: e, perfil: profileWithTimeSelections.perfils }
                    })
            ]
            groupCursos = [
                ...groupCursos,
                ...
                profileWithTimeSelections.timeSelections.filter(x => x.variacao == 3)
                    .map(e => {
                        return { ts: e, perfil: profileWithTimeSelections.perfils }
                    })
            ]

        }

        groupOneToOne.sort(compararPorDataDecrescente);
        groupCursos.sort(compararPorDataDecrescente);



        for (let item in groupOneToOne)
            preparedOneToOne += JoinTimeForMentorias(groupOneToOne[item].ts, groupOneToOne[item].perfil, "groupOneToOne", item)
        for (let item in groupCursos)
            preparedCursos += JoinTimeForMentorias(groupCursos[item].ts, groupCursos[item].perfil, "groupCursos", item)

        $("#OneToOneMatch").html(preparedOneToOne)
        if (groupOneToOne.length > 0) {
            countOneToOneElement.html(groupOneToOne.length).addClass("count-tab-events")
        }

        $("#CursosMatch").html(preparedCursos)
        if (groupCursos.length > 0) { countCursosElement.html(groupCursos.length).addClass("count-tab-events") }

        countOneToOne = groupOneToOne.length;
        countCursos = groupCursos.length;
        activePaginationFor("groupOneToOne", groupOneToOne.length)
        activePaginationFor("groupCursos", groupCursos.length)
        
    }
    countLoaded++
    countLoaded++

    updateOpenPanel()

}

function loadPedidosAjudaParaTag(data) {
  let prepared = "";
  if (data.length == 0) {
    prepared =
      "<div style='justify-content: center; display: flex; padding: 20px; color: grey;'>Sem pedidos de ajuda no momento </div>";
    $("#pedidosPanel").html(prepared);
    activePaginationFor("groupOneToOne", 0);
    activePaginationFor("groupCursos", 0);
    activePaginationFor("groupHelp", 0);
  } else {
    let pedidosDeAjuda = "";
    let groupHelp = [];
    for (const profileWithTimeSelections of data) {
      result.push(profileWithTimeSelections);
      groupHelp = [
        ...groupHelp,
        ...profileWithTimeSelections.timeSelections
          .filter((x) => x.variation == 5)
          .map((e) => {
            return { ts: e, perfil: profileWithTimeSelections.perfils };
          }),
      ];
    }



        groupHelp.sort(compararPorDataDecrescente);

        for (let item in groupHelp)
            pedidosDeAjuda += JoinTimeForRequestedHelp(groupHelp[item].ts, groupHelp[item].perfil, "groupHelp", item)

        $("#pedidosPanel").html(pedidosDeAjuda)
        activePaginationFor("groupHelp", groupHelp.length)
        if (groupHelp.length > 0) { countHelpElement.html(groupHelp.length).addClass("count-tab-events") }
        countHelp = groupHelp.length;

       
        
    }
    countLoaded++
    updateOpenPanel()

}


function loadMentores(data) {
    let prepared = ""
    if (data.length == 0) {
        $("#mentoriaBoard").html("Nenhuma mentoria encontrada no momento :/")
        return;
    }
    for (const profileWithTimeSelections of data) {
        prepared += PrepareMentorCard(profileWithTimeSelections, "Preview")
    }
    $("#mentoriaBoard").html(prepared)
}

// function loadRequestedHelp(data) {
//     let prepared = ""
//     if (data.length == 0) {
//         $("#ajudaPedidaBoard").html("Nenhum pedido de ajuda encontrada no momento :/")
//         return;
//     }
//     for (const profileWithTimeSelections of data) {
//         result.push(profileWithTimeSelections)
//         prepared += PrepareRequestedHelpCard(profileWithTimeSelections, "Preview")
//     }
//     $("#ajudaPedidaBoard").html(prepared)
// }
function activePaginationFor(group, length){
    const left = document.querySelector(`#${group}-left`);
    const right = document.querySelector(`#${group}-right`);
    let page = 1;
    const size = 2;
    const lastPage = Math.ceil(length / size);

    if(length <= size){
        return;
    }

    right.style.filter = "grayscale(0)";

    function updatePagination() {

        const start = (page - 1) * size;
        const end = Math.min(start + size, length);


        const items = document.querySelectorAll(`.${group}`);
        items.forEach(item => {
            item.style.display = 'none';
        });

        for (let i = start; i < end; i++) {
            items[i].style.display = 'flex';
        }

        left.style.filter = (page === 1) ? "grayscale(0.8)" : "grayscale(0)";
        right.style.filter = (page === lastPage) ? "grayscale(0.8)" : "grayscale(0)";
    }

    // Evento para avançar para a próxima página
    right.addEventListener("click", e =>{
        if(page < lastPage) {
            page++;
            updatePagination();
        }
    });

    // Evento para retroceder para a página anterior
    left.addEventListener("click", e =>{
        if(page > 1) {
            page--;
            updatePagination();
        }
    });

    // Inicializa a paginação
    updatePagination();
}
function compararPorDataDecrescente(a, b) {
    return new Date(a.ts.startTime) - new Date(b.ts.startTime);
}
