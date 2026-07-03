(function (window, $) {
  "use strict";

  function optionText(option) {
    return (option.textContent || option.innerText || "").trim();
  }

  function optionValue(option) {
    return (option.value || "").trim();
  }

  function cleanupSelectElement(select) {
    if (!select || !select.options) {
      return;
    }

    var seen = Object.create(null);
    var emptyPlaceholderKept = false;
    var toRemove = [];

    for (var i = 0; i < select.options.length; i++) {
      var opt = select.options[i];
      var text = optionText(opt);
      var value = optionValue(opt);

      if (text === "" && value !== "") {
        toRemove.push(opt);
        continue;
      }

      if (text === "" && value === "") {
        if (emptyPlaceholderKept) {
          toRemove.push(opt);
        } else {
          emptyPlaceholderKept = true;
        }
        continue;
      }

      var key = value + "\u0000" + text;
      if (seen[key]) {
        toRemove.push(opt);
      } else {
        seen[key] = true;
      }
    }

    toRemove.forEach(function (opt) {
      if (opt.parentNode) {
        opt.parentNode.removeChild(opt);
      }
    });
  }

  function cleanupSelectTree(root) {
    var scope = root || document;
    $(scope)
      .find("select")
      .addBack("select")
      .each(function () {
        cleanupSelectElement(this);
      });
  }

  function shouldUseSelect2($el) {
    return (
      $el.hasClass("select2bs4") ||
      $el.hasClass("select2-medicos-modal") ||
      $el.hasClass("select2-paquetes-destino") ||
      $el.hasClass("select2-personal")
    );
  }

  function refreshSelect2($el) {
    if (!$el || !$el.length || !shouldUseSelect2($el)) {
      return;
    }

    var currentValue = $el.val();
    if ($el.data("select2")) {
      $el.trigger("change.select2");
      return;
    }

    var config = {
      theme: "bootstrap4",
      width: "100%",
    };

    var dropdownParent = $el.closest(".modal");
    if (dropdownParent.length) {
      config.dropdownParent = dropdownParent;
    }

    $el.select2(config);
    if (currentValue !== null && currentValue !== undefined) {
      $el.val(currentValue).trigger("change.select2");
    }
  }

  function filterItemsForSelect(items, textKey) {
    if (!Array.isArray(items)) {
      return [];
    }

    return items.filter(function (item) {
      if (item == null) {
        return false;
      }

      var text =
        typeof textKey === "function"
          ? textKey(item)
          : item[textKey];

      return text != null && String(text).trim() !== "";
    });
  }

  window.SelectHelpers = {
    cleanupSelectElement: cleanupSelectElement,
    cleanupSelectTree: cleanupSelectTree,
    refreshSelect2: refreshSelect2,
    filterItemsForSelect: filterItemsForSelect,
  };

  function patchKnockoutOptionsBinding() {
    if (!window.ko || !ko.bindingHandlers || !ko.bindingHandlers.options) {
      return;
    }

    var originalUpdate = ko.bindingHandlers.options.update;
    if (originalUpdate && originalUpdate.__selectHelpersPatched) {
      return;
    }

    ko.bindingHandlers.options.update = function (
      element,
      valueAccessor,
      allBindingsAccessor,
      viewModel,
      bindingContext
    ) {
      if (originalUpdate) {
        originalUpdate.call(
          this,
          element,
          valueAccessor,
          allBindingsAccessor,
          viewModel,
          bindingContext
        );
      }

      cleanupSelectElement(element);

      if (window.jQuery) {
        var $element = $(element);
        if ($element.data("select2")) {
          $element.trigger("change.select2");
        } else {
          refreshSelect2($element);
        }
      }
    };

    ko.bindingHandlers.options.update.__selectHelpersPatched = true;
  }

  $(function () {
    cleanupSelectTree(document);
    patchKnockoutOptionsBinding();
  });
})(window, window.jQuery);
