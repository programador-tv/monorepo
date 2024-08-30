// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

function renderAvatar(perfil, withLink) {
  if (withLink) {
    return `
        <a href='/Canal/Index?usr=${perfil.userName}'
        title="${perfil.userName}"
        target="_blank"
        style="cursor: default; text-decoration: none; color: inherit; margin-right: 8px;"
        >
            <img
            onerror="if (this.src != '/no-user.svg') this.src = '/no-user.svg';"
            src="/${perfil.foto}"
            class="link-avatar" style="width: 44px; height: 44px; border-radius: 50%;">
        </a>
        `;
  } else {
    return `
        <img
        onerror="if (this.src != '/no-user.svg') this.src = '/no-user.svg';"
        src="/${perfil.foto}"
        class="avatar-sm">
        `;
  }
}
