let calendar;
let rendered = false;
let now = new Date();
let today = new Date(now.getFullYear(), now.getMonth(), now.getDate());


document.addEventListener('DOMContentLoaded', function () {

    $('#calendarModal').on('show.bs.modal', function () {
        setTimeout(render,200)
    })
})


let calendarEl = document.getElementById('calendar');
const conf = {
    nowIndicator: true,
    scrollTime: now.toLocaleString().slice(11, 19),
    initialView: 'timeGridWeek',
    selectable: true,
    locale: 'pt-br',
    allDaySlot: false,
    slotDuration: '01:00:00',
    buttonText: {
        today: 'Hoje'
    },
    eventTextColor: 'black',
    height: '700',
    dayHeaderContent: function (info) {
        let date = info.date
        let dateStr = date.toLocaleDateString('pt-BR', { day: 'numeric' });
        let dayOfWeekStr = date.toLocaleDateString('pt-BR', { weekday: 'short' });
        return dateStr + ' ' + dayOfWeekStr;
    },
    events: [],

    eventClick: function (info) {

        $('#eventModal-' + info.event.id).modal('show');
    },
    views: {
        timeGridWeek: {
            dayCellDidMount: function (cell) {
                let currentDate = new Date();
                if (cell.date < new Date(currentDate.getFullYear(), currentDate.getMonth(), currentDate.getDate(), currentDate.getHours(), currentDate.getMinutes())) {
                    cell.el.style.backgroundColor = '#E6E6E6';
                }
                if (cell.date.toDateString() === currentDate.toDateString()) {
                    cell.el.style.backgroundColor = '#E1F2FA';
                }
            }
        }
    },
    headerToolbar: {
        end: "prev today next",
        },

}


    // if (window.location.href.indexOf("Agenda") > -1) {
    //     let events = calendar.getEvents();
    //     let agenda = window.location.href.split("Agenda=")[1];
    //     for (let item of events) {
    //         if (item.id == agenda) {
    //             let startTime = item.start;
    //             let hours = startTime.getHours();
    //             let minutes = startTime.getMinutes();

    //             // Formate a hora e os minutos como uma string
    //             let formattedTime = hours.toString().padStart(2, '0') + ':' + minutes.toString().padStart(2, '0');

    //             const el = $("#events-tab")
    //             let firstTab = new bootstrap.Tab(el)

    //             firstTab.show()

    //             render();

    //             calendar.scrollToTime(formattedTime);

    //             $('#eventModal-' + agenda).modal('show');
    //         }
    //     }
    // }

calendar = new FullCalendar.Calendar(calendarEl, conf);

function render() {
    if (rendered) {
        return;
    }

    calendar.render();

    rendered = true;
}

function RenderCalendarItens() {
    const url = "/ScheduleActions?handler=RenderCalendar";

    fetch(url)
      .then((response) => response.json())
      .then(async (data) => {

        const timeSelection = [... data.myTimeSelection, ... data.attachedTimeSelection];
        const badFinished = data.badFinished;

        await RenderActualEvents(timeSelection,data.modals, data.joinTimeModalsPanel, data.timeSelectionPanelModals, data.requestHelpModalsPanel, data.solvedHelpModalsPanel);

        for (let value of badFinished) {
            RenderOldEvents(value);
        }
      })
      .catch((error) => {
        console.error("Ocorreu um erro:", error);
      });
  }

async function RenderActualEvents(events, embed, joinTimes, TimeSelectionModals, requestHelpList, solvedHelpList) {

    for(let event of events){

      const eventStyle = { id: event.id, title: event.title, start: event.start, end: event.end };
      if (event.actionNeeded) {
          eventStyle.backgroundColor = '#FFC107';
      } else if (event.status === 1) {
          eventStyle.backgroundColor = '#D1DDE6';
      } else if (event.status === 3) {
          eventStyle.backgroundColor = '#9B91DB';
      } else if (event.tipo === 0) {
          eventStyle.backgroundColor = 'rgba(222, 164, 156, 0.45)';
      } else {
          eventStyle.backgroundColor = 'white';
      }
      calendar.addEvent(eventStyle);
      asyncEvents.push(eventStyle);
      }

    $("#eventModals").html(embed)
    let needOpenModalId;

    $("#joinTimes").html(joinTimes)

    $("#freeTimes").html(TimeSelectionModals)
    if (window.location.href.indexOf("event") > -1) {
        needOpenModalId = window.location.href.split("event=")[1];
        $("#calendarModal").modal("show");
        $("#eventModal-" + needOpenModalId).modal("show");
    }

    $("#askedHelp").html(requestHelpList)
    if (window.location.href.indexOf("event") > -1) {
        needOpenModalId = window.location.href.split("event=")[1];
        $("#calendarModal").modal("show");
        $("#eventModal-" + needOpenModalId).modal("show");
    }

    $("#solvedHelp").html(solvedHelpList)
    if (window.location.href.indexOf("event") > -1) {
        needOpenModalId = window.location.href.split("event=")[1];
        $("#calendarModal").modal("show");
        $("#eventModal-" + needOpenModalId).modal("show");
    }
  }

  function RenderOldEvents(oldEvent) {
    const eventStyle = {
      textColor: "white",
      backgroundColor: "#838383",
      id: oldEvent.id,
      title: oldEvent.title,
      start: oldEvent.start,
      end: oldEvent.end,
    };

    if (oldEvent.tipo === 0) {
      eventStyle.backgroundColor = "#DEA49C";
    }

    asyncEvents.push(eventStyle);
    calendar.addEvent(eventStyle);

  }
