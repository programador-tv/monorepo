function createSelect2(selector) {
  $(selector).select2({
    maximumSelectionLength: 5,
    closeOnSelect: true,
    placeholder: "Tags",
    tags: true,
  });
  $(selector).on("change", function (e) {
    const selectedOptions = $(this).find(":selected");
    const container = $("#selected-tags-container");
    container.empty(); // Limpa o contêiner antes de adicionar os novos spans

    selectedOptions.each(function (index) {
      const optgroupLabel = $(this).parent("optgroup").attr("label");
      const selectedText = $(this).text();
      let textoSelecionado;

      // Verifica se o texto já inclui o rótulo do grupo
      if (!selectedText.includes(optgroupLabel)) {
        textoSelecionado = optgroupLabel + " : " + selectedText;
      } else {
        textoSelecionado = selectedText;
      }

      $(this).val(textoSelecionado);
      $(this).text(textoSelecionado);
      const span = $("<span>").text(textoSelecionado).addClass("selected-tag");
      container.append(span);
    });
  });
}
